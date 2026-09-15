# Decisions Log (Gunvalanche)

Этого файла раньше не было, поэтому первая часть записей — не решения "как было задумано",
а **реконструкция по коду** на 2026-09-14 (аудит ветки `refactor/stabilize`). Если реальная
причина решения была другой — поправь запись, не удаляя историю (допиши "Update:").

## Как пользоваться файлом

- Добавляй новую запись **сверху** соответствующего раздела, не переписывай старые задним числом.
- Формат записи:
  ```
  ### [Статус] Заголовок (YYYY-MM-DD)
  Контекст: почему вообще встал вопрос.
  Решение: что сделали.
  Альтернативы: что не взяли и почему (если было).
  Последствия: чем расплачиваемся / что теперь нельзя делать иначе.
  ```
- Статусы: `Accepted`, `Reconstructed` (восстановлено по коду задним числом, не проверено с автором),
  `Superseded by <ссылка>`, `Open` (известная проблема без решения).
- Если решение отменяет старое — не удаляй старое, добавь `Superseded by ...` в его статус.

---

## 1. Архитектура рантайма

### [Accepted] Фикс: краш на ESC после рестарта уровня — висящая подписка на Pause (2026-09-15)
Контекст: после смерти → Retry → рестарта сцены, нажатие ESC иногда кидало
`MissingReferenceException` на `GameOverUI` (repro и полный стек — см. сессию чата).
По логам с instance id выяснилось: `current` state оказывался `GameOverState` не потому,
что `GameStateMachine` пережил перезагрузку сцены (не пережил, подтверждено — новый
instance id при каждом рестарте), а потому что **делегат `Pauser.OnPausePerformed` со
старой сцены оставался подписан** на Pause-экшен и продолжал срабатывать вместе с новым.
Причина: `Pauser.Unregister()` заново доставал `InputAction` через
`inputManager.GetComponent<PlayerInput>()`, а порядок уничтожения объектов при выгрузке
сцены не гарантирован — если `InputManager` игрока уничтожался раньше `Pauser`, проверка
`inputManager == null` тихо обрывала отписку.
Отдельно нашлась вторая, независимая причина, почему `current` вообще оказывался
`GameOverState`: `PlayerDeathHandler.OnPlayerDied` шлёт и `Died` (instance-событие), и
`PlayerDied` (static), `PlayerDeathObserver` подписан на оба — `GameOver` запускался
дважды подряд за один кадр.
Решение:
1. `Pauser.cs` — кэширует `InputAction` в момент подписки (`subscribedPauseAction`),
   отписывается через этот кэш, а не через повторный `GetComponent`. Не зависит от того,
   жив ли ещё `inputManager` на момент `OnDestroy()`.
2. `GameStateController.SetState()` — идемпотентность: повторный запрос того же
   состояния — no-op. Убирает двойной `GameOver` и защищает от подобного дублирования
   в любом другом переходе в будущем.
3. `GameOverState.cs`/`GameplayState.cs` — обращения к `GameOverUI` переведены на `?.`
   (было несогласованно: `PauseUI` уже был защищён `?.`, `GameOverUI` — нет). Второй
   рубеж защиты на случай похожей утечки подписки где-то ещё.
Файлы: `Pauser.cs`, `GameStateController.cs`, `GameOverState.cs`, `GameplayState.cs`.

### [Accepted] Фикс: дублирование оружия — PlayerSwitchWeapon.CollectWeapon() вызывался не только как fallback (2026-09-15)
Контекст: у игрока с начала уровня было 2 пистолета (слот 1 и слот 2 колёсика мыши),
хотя `PlayerInventory` выдаёт стартовый пистолет ровно один раз (подтверждено логом:
`AddWeapon`/`Registered weapon` вызывались по одному разу каждый, но со `index=1`, то
есть в списке уже был 1 элемент до этого вызова).
Причина — в `PlayerSwitchWeapon.Start()`: код и комментарий расходились. Warning-текст
`"Fallback to initial scan only"` прямо говорил, что `CollectWeapon()` (сканирование
всей иерархии `weaponsHolder` на `IWeapon`-компоненты) — это fallback на случай, если
`PlayerInventory` не найден. Но по факту `CollectWeapon()` стоял ПОСЛЕ `if/else`, а не
внутри `else` — вызывался всегда. Получалось два параллельных источника правды для
одного и того же списка оружия: событие `OnWeaponAdded → RegisterWeapon()` (с проверкой
на дубликаты) и отдельное безусловное сканирование `CollectWeapon()` (без проверки).
Проверено и исключено: baked-in пистолет в `Player.prefab` (0 вхождений
`WeaponController`), дублирующийся компонент `WeaponController` в `Pistol.prefab` (ровно
1 вхождение) — то есть дубль создавался именно логикой `PlayerSwitchWeapon`, не данными.
Решение: `CollectWeapon()` + первоначальный `SwitchWeaponByIndex(0)` перенесены внутрь
`else`-ветки, где по смыслу и должны были быть — работают только когда `PlayerInventory`
не найден. Когда `PlayerInventory` есть (обычный случай), единственный источник —
событие `OnWeaponAdded`.
Файл: `Assets/Scripts/Player/Player&Gun/PlayerSwitchWeapon.cs`.

### [Reconstructed] Явная State Machine для потока игры (2026-09-14)
Контекст: нужно было единообразно переключать Gameplay / Pause / GameOver, включая
курсор, input action map и видимость HUD.
Решение: `GameStateMachine` (MonoBehaviour, входная точка) → `GameStateController`
(чистый C#, хранит `Dictionary<GameStateId, IGameState>`) → состояния `GameplayState`,
`PausedState`, `GameOverState`, которые дергают `GameStateContext` (общий "bag" зависимостей:
UI-экраны, InputManager, колбэки на смену action map).
Файлы: `Assets/Scripts/App/GameFlowCore/GameStateMachine/*`.
Последствия: чтобы добавить новое состояние — реализовать `IGameState` и зарегистрировать
в `GameStateController`. `GameStateContext` умеет лениво резолвить `HPAmmoVisibilityController`
и `WeaponIconVisibilityController` через `FindObjectOfType`, если ссылка не проставлена в
инспекторе — это осознанный fallback, а не забытый баг.

### [Reconstructed] Object pooling — по одному `ObjectPool` на тип объекта, без единого менеджера (2026-09-14)
Контекст: пули и враги переиспользуются, не создаются/уничтожаются каждый раз.
Решение: `ObjectPool` (MonoBehaviour, `Queue<GameObject>`, растёт через `ExpandPool()` без
верхнего предела) создаётся отдельным префабом на каждый тип: `AmmoManager` инстанцирует
4 пула под виды пуль (`pistolBulletsPool`, `riffleBulletsPool`, `shotgunBulletsPool`,
`rocketsPool`), `EnemySpawner` получает ссылку на `enemyPool` напрямую через инспектор.
Альтернативы: был раньше общий `ObjectPoolManager` с реестром пулов по строковому ID —
код остался в репозитории **полностью закомментированным**
(`Assets/Scripts/ObjectPool/ObjectPoolManager.cs`, `PoolableObject.cs`), от него отказались
в пользу прямых ссылок. Файлы — мёртвый груз, стоит либо удалить, либо, если идея ещё
рассматривается, дописать сюда почему вернулись к прямым ссылкам.
Последствия: нет общего лимита/статистики по всем пулам сразу; при затяжном бою пулы
только растут (см. Open-issue ниже).

### [Superseded by запись ниже] Конфиги оружия — ScriptableObject, но с "плавающим" рантайм-состоянием (2026-09-14)
Контекст: нужно было где-то хранить статичные данные оружия (урон, magazine size, префабы)
и per-run состояние (`CurrentAmmo`, `TotalAmmo`).
Решение (как было до фикса): `WeaponConfig : ScriptableObject` хранил `WeaponData[]` — а
`WeaponData` это **class**, то есть рантайм-мутации (`weaponData.CurrentAmmo--` в
`WeaponController.Shoot()`) писались прямо в поля ассета, а не в отдельную копию. Чтобы
ассет не оставался "грязным" между запусками Play Mode в редакторе, `WeaponConfig` сам
подписывался на `EditorApplication.playModeStateChanged` и откатывал `CurrentAmmo`/
`TotalAmmo` на `ExitingPlayMode`.
Последствия: это было узкое исправление симптома только для Editor Play-кнопки — не помогало
при обычном рестарте уровня в билде. См. фикс ниже — Open-issue #1 закрыт.

### [Accepted] Патроны оружия вынесены из WeaponData(ScriptableObject) в per-instance `WeaponRuntimeData` (2026-09-14)
Контекст: см. запись выше — `CurrentAmmo` хранился прямо в живом ассете `WeaponConfig`,
из-за чего магазин мог не сбрасываться при рестарте уровня (`AmmoManager` грузит `WeaponConfig`
через `Resources.LoadAll`, такие ассеты переживают обычную перезагрузку сцены).
Решение: `WeaponData.CurrentAmmo`/`MagazineSize` остаются как есть — это теперь просто
design-time дефолт ("сколько патронов в обойме при заходе на уровень"), в него больше никто
не пишет. Реальное мутируемое состояние живёт в новом `WeaponRuntimeData` — обычном C#-объекте,
который `WeaponController.Awake()` создаёт заново на каждый инстанс оружия, засеяв значением
из `weaponData.CurrentAmmo` (или `MagazineSize`, если 0). `IWeapon` получил метод
`AddAmmoToMagazine(int amount)` — через него `PlayerReload` пополняет магазин, не трогая
`WeaponData` напрямую. Заодно убран весь editor-хак из `WeaponConfig`
(`OnPlayModeStateChanged`/`ResetToDefaultRuntimeData`) — он больше не нужен, потому что
мутировать состояние ассета теперь просто негде в коде.
Файлы: `WeaponRuntimeData.cs` (переписан), `WeaponConfig.cs` (упрощён до чистого
контейнера данных), `WeaponController.cs`, `IWeapon.cs`, `PlayerReload.cs`,
`PlayerAutoReload.cs`, `RocketChamberPresenter.cs`.
Последствия: `WeaponData.TotalAmmo` (резерв патронов) не трогали — этот счётчик и так жил
только в `AmmoManager.ammoStorage` (Dictionary на MonoBehaviour-синглтоне), пересоздаваемом
заново при каждой загрузке сцены, и никогда не писался обратно в ассет — там утечки не было.
Не проверено вручную в редакторе/билде — рекомендуется прогнать плейтест "умереть → Retry"
и убедиться, что магазин каждый раз стартует с дефолтного значения.

### [Reconstructed] Разблокировка оружия по уровню + принудительная разблокировка (2026-09-14)
Контекст: не всё оружие доступно с самого начала, часть подбирается/открывается по мере
прогресса.
Решение: `WeaponUnlockManager` хранит `currentLevel` и таблицу `WeaponUnlockEntry`
(`gunsType → levelToUnlock, spawnAllowed`). `IsUnlocked()` разрешает оружие, если
`currentLevel >= levelToUnlock` **или** оно принудительно открыто через `UnlockNow()`
(используется, например, когда игрок явно подбирает предмет). `IsAllowedToSpawn()` —
отдельная проверка "можно ли этому оружию вообще появляться на уровне сейчас", независимая
от факта разблокировки.
Файлы: `Assets/Scripts/Weapons/WeaponCore/WeaponUnlockManager.cs`.
Последствия: `unlockedWeapons` (HashSet, только в памяти) не персистится между уровнями/сессиями
сам по себе — если нужна сквозная прогрессия между сценами, это должно приходить через
`Context`/`DataSystem`, а не через этот компонент.

### [Reconstructed] Input — New Input System, переключение action map через события, а не прямые вызовы (2026-09-14)
Контекст: нужно блокировать геймплейный ввод, когда открыт UI (пауза/game over), и наоборот.
Решение: `InputManager` (обёртка над `PlayerInput`) предоставляет типизированные `InputAction`
свойства и `SwitchToGameplayActionMap()/SwitchToUIActionMap()`. `ActionMapRequester` — тонкая
прослойка, которая держит "отложенный" запрос на смену карты (`pendingActionMap`), если
`InputManager` ещё не готов (актуально до того, как заспавнится игрок) и подписывается на
`PlayerSpawner.OnPlayerSpawned`, чтобы подхватить `InputManager` игрока сразу после спавна.
Файлы: `ActionMapRequester.cs`, `InputSystem/InputManager.cs`.
Последствия: имена action map'ов (`"GameplayActionMap"`, `"UIActionMap"`) сравниваются как
строки в двух разных местах (`ActionMapRequester.RequestActionMap` и `InputManager`
константы) — при переименовании в `.inputactions`-ассете нужно поправить оба места руками,
компилятор не подскажет.

### [Reconstructed] Иконки оружия — два взаимозаменяемых провайдера (Resources / Addressables) (2026-09-14)
Контекст: судя по `git log` (`add addressables`), проект начинался на `Resources.Load`,
затем часть перевели на Addressables, но не полностью.
Решение: `IWeaponIconProvider` реализован в двух вариантах — `ResourcesWeaponIconProvider`
и `AddressablesWeaponIconProvider`, конкретный назначается в инспекторе `WeaponIconManager`
через поле `MonoBehaviour providerComponent` (не строгую ссылку на интерфейс — так его можно
провалидировать в инспекторе). Если провайдер вообще не назначен, `WeaponIconManager` молча
падает обратно на прямой `Resources.Load<Sprite>($"WeaponIcons/{iconKey}")`.
Последствия: в проекте фактически два конкурирующих способа доставки иконок; при добавлении
нового оружия иконку нужно либо класть в `Resources/WeaponIcons`, либо регистрировать в
Addressables — в зависимости от того, какой провайдер стоит на конкретной сцене/префабе UI.
Явного единого стандарта в коде не закреплено.

---

### [Accepted] Git LFS: полная миграция истории (main + 4 активные ветки) (2026-09-15)
Контекст: `.git` весил ~924 МБ без Git LFS, тяжёлые бинарники (текстуры Portal Collection,
`Forest.hdr`, FBX-модели) были закоммичены напрямую в историю. См. запись выше про
`git lfs migrate` — не просто настройка на будущее, а именно переписывание уже
существующей истории.
Решение: смигрировали `*.png,*.jpg,*.jpeg,*.tga,*.tif,*.tiff,*.psd,*.exr,*.hdr,*.dds,*.bmp,
*.gif,*.wav,*.mp3,*.ogg,*.aiff,*.mp4,*.mov,*.fbx,*.obj,*.blend,*.max,*.ma,*.mb,*.ttf,*.otf,
*.unitypackage` (~516 МБ, 158 коммитов) в 5 веток: `main`, `refactor/stabilize`,
`AmmoBoxes`, `InputSystem`, `Pause`. 32 уже смёрженные ветки сознательно не трогали — их
содержимое и так полностью внутри `main`, переписывать нечего, только риск. Перед миграцией
сняли полный mirror-бэкап (`git clone --mirror`) в `D:\Unity\Projects\Gunvalanche_BACKUP_before_lfs.git`
— на случай отката. После миграции все 5 веток force-push'нуты на origin.
Важные грабли по пути (см. также вывод сессии):
- `git lfs migrate import` без `--everything` по умолчанию трогает только коммиты,
  которых ещё нет ни на одном remote — первая попытка почти ничего не мигрировала
  (переписала только 2 локальных ещё не запушенных коммита). С `--everything` заработало
  верно (158 коммитов).
- После миграции локальные файлы в рабочей копии стали LFS-pointer'ами (текст вместо
  бинарника), пока не запустили `git lfs checkout` — фильтр `smudge` не применился
  автоматически к уже вычекаученным файлам сразу после `migrate import`. После одного
  `git lfs checkout` обычные `git checkout` между ветками дальше смуджат корректно сами.
Последствия: `.git` локально временно вырос (~2.8 ГБ) — старые (домиграционные) объекты
всё ещё физически на диске, пока не сделан `git gc --prune=now` (не срочно). Любой другой
клон этого репозитория (другая машина) должен быть пересоздан или сделать
`git fetch && git reset --hard origin/<branch>` — историю переписали, старые локальные
копии разъедутся с origin. `git-lfs` должен быть установлен (`git lfs install`) на каждой
машине, которая будет с этим репозиторием работать.

## 2. Известные проблемы / открытые вопросы (из аудита 2026-09-14)

### [Closed 2026-09-14] #1 — Патроны оружия хранились в живом ScriptableObject-ассете
Смотри раздел 1, запись "Патроны оружия вынесены из WeaponData(ScriptableObject) в
per-instance `WeaponRuntimeData`" — там и решение, и что именно поменялось.

### [Open, частично закрыто 2026-09-15] #2 — `ObjectPool.ExpandPool()` растёт без верхнего предела
`Assets/Scripts/ObjectPool/ObjectPool.cs:87-97`. Сам факт "пул растёт и не уменьшается
обратно" — осознанно не трогали, это trade-off (простота > память), решение по нему пока
отложено (`maxPoolSize` с предупреждением так и не заведён).
Но конкретный практический случай — реальный warning `ObjectPool: ShotgunBulletsPool is
empty, expanced pool.` в игре на 2-й волне — был не про архитектуру пула, а про
неправильный расчёт начального размера. `ShotgunBulletController.pelletsPerShot = 8`, а
`ShotgunBulletsPool.poolSize` был выставлен в 18 — то есть при полном расстреле магазина
(`MagazineSize: 6` в `Shotgun.asset`) в полёте одновременно может быть 6×8=48 дробинок
(с учётом `LifeTime: 4` в `BulletsConfig.asset` и перезарядки за 2с — дробинки предыдущего
магазина ещё живы, когда начинают лететь новые). 18 < 48 — пул неизбежно исчерпывался.
Для сравнения: `PistolBulletsPool` (10, магазин 7×1) и `RiffleBulletsPool` (31, магазин
30×1) — пул сопоставим с магазином, там всё верно, проблема была именно в том, что для
дробовика никто не учёл множитель "8 дробинок за выстрел" при выставлении `poolSize`.
Решение: `ShotgunBulletsPool.prefab` → `poolSize: 18` → `64` (48 = точный худший случай,
64 — с запасом на перекрытие при быстрой перезарядке). Сама архитектурная часть
(отсутствие верхнего предела у `ExpandPool()`) остаётся открытой — см. описание выше.

### [Open] #3 — `EnemyHealthController` создаёт damage-text без владельца жизненного цикла
`Assets/Scripts/Enemy/EnemyHealthController.cs:17-26` — `Instantiate(damageTextPrefab, ...)`
без Destroy/пула. Сейчас не проявляется, т.к. поле не назначено в `WeakEnemy.prefab`, но
это тикающая бомба на момент, когда кто-то подключит префаб текста урона.

### [Closed 2026-09-15] #4 — Репозиторий без Git LFS, ~924 МБ `.git`
Смотри раздел 1, запись "Git LFS: полная миграция истории (main + 4 активные ветки)".

### [Open] #5 — Мёртвый код в репозитории
`ObjectPool/ObjectPoolManager.cs`, `ObjectPool/PoolableObject.cs` — полностью закомментированы;
`Assets/BigRookGames/...GunfireController.cs` — демо-скрипт ассета, не используется ни в одной
сцене/префабе, при этом содержит баг (двойной `Instantiate(source)`, первый экземпляр течёт).
Решение пока не принято: удалить или оставить как референс.

---

## Как добавлять новые записи

Когда принимаете архитектурное решение (новая система спавна, смена подхода к сохранениям,
переход на другой namespace для DI и т.п.) — добавьте запись сюда **в момент решения**,
а не постфактум. Три-пять строк контекста экономят часы реверс-инжиниринга в следующий раз.
