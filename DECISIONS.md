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

## 2. Известные проблемы / открытые вопросы (из аудита 2026-09-14)

### [Closed 2026-09-14] #1 — Патроны оружия хранились в живом ScriptableObject-ассете
Смотри раздел 1, запись "Патроны оружия вынесены из WeaponData(ScriptableObject) в
per-instance `WeaponRuntimeData`" — там и решение, и что именно поменялось.

### [Open] #2 — `ObjectPool.ExpandPool()` растёт без верхнего предела
`Assets/Scripts/ObjectPool/ObjectPool.cs:87-97`. При долгих волнах врагов / плотном отстреле
ракет пул может разрастись и не уменьшается обратно. Нужно либо залогировать это как
осознанный trade-off (простота > память), либо завести `maxPoolSize` с предупреждением.

### [Open] #3 — `EnemyHealthController` создаёт damage-text без владельца жизненного цикла
`Assets/Scripts/Enemy/EnemyHealthController.cs:17-26` — `Instantiate(damageTextPrefab, ...)`
без Destroy/пула. Сейчас не проявляется, т.к. поле не назначено в `WeakEnemy.prefab`, но
это тикающая бомба на момент, когда кто-то подключит префаб текста урона.

### [Open] #4 — Репозиторий без Git LFS, ~924 МБ `.git`
Тяжёлые бинарники (`Portal*.png/tga`, `Forest.hdr` и т.д.) закоммичены напрямую. Нужно решить:
переходим на Git LFS сейчас (миграция истории или хотя бы новых файлов) или сознательно
остаёмся без LFS — и почему.

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
