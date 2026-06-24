export type AppLanguage = "uk" | "en";

const STORAGE_KEY = "sporthive_lang";

const EN: Record<string, string> = {
  // navigation
  "Головна": "Home",
  "Турніри": "Tournaments",
  "Статистика": "Statistics",
  "Статистика організації": "Organization statistics",
  
  "Створити захід": "Create event",
  "Профіль Організації": "Organization profile",
  "Профіль організації": "Organization profile",
  "Мій профіль": "My profile",
  "Мій кабінет": "My account",
  "Профіль": "Profile",
  "Увійти": "Sign in",
  "Зареєструватись": "Sign up",
  "Вийти": "Log out",
  "Створити команду": "Create team",
  "N/A": "N/A",

  // common
  "Назад": "Back",
  "Оновити": "Refresh",
  "Додати": "Add",
  "Видалити": "Remove",
  "Редагувати": "Edit",
  "Редагувати команду": "Edit team",
  "Зберегти": "Save",
  "Скасувати": "Cancel",
  "Пошук": "Search",
  "Деталі": "Details",
  "Перегляд": "View",
  "Переглянути": "View",
  "Переглянути профіль": "View profile",
  "Переглянути команду": "View team",
  "Внести": "Enter",
  "Завантаження...": "Loading...",
  "Помилка": "Error",
  "Не вдалося завантажити": "Could not load",
  "Нічого не знайдено": "Nothing found",
  "Опис відсутній": "No description",
  "Опис організації поки не заповнений.": "Organization description is not filled yet.",
  "Матчів поки немає": "No matches yet",
  "Команд поки немає": "No teams yet",
  "Організацій поки немає": "No organizations yet",
  "Спортсменів поки немає": "No athletes yet",
  "Суддів поки немає": "No judges yet",
  "Тренерів поки немає": "No trainers yet",
  "Подій поки немає": "No events yet",
  "Запрошень поки немає": "No invitations yet",
  "Даних для таблиці поки немає": "No table data yet",

  // roles
  "Організація": "Organization",
  "Організації": "Organizations",
  "Тренер": "Trainer",
  "Тренери": "Trainers",
  "Суддя": "Judge",
  "Судді": "Judges",
  "Спортсмен": "Athlete",
  "Спортсмени": "Athletes",
  "Атлети": "Athletes",
  "Атлети організації": "Organization athletes",
  "Команда": "Team",
  "Команди": "Teams",
  "Учасник": "Participant",
  "Учасники": "Participants",

  // registration/auth
  "Реєстрація": "Registration",
  "Email:": "Email:",
  "Email": "Email",
  "Логін:": "Login:",
  "Логін": "Login",
  "Пароль:": "Password:",
  "Пароль": "Password",
  "Оберіть роль:": "Choose role:",
  "Оберіть роль": "Choose role",
  "Далі": "Next",
  "Підтвердження Email": "Email confirmation",
  "Підтвердження email": "Email confirmation",
  "Код:": "Code:",
  "Код": "Code",
  "Підтвердити": "Confirm",
  "Заповніть профіль": "Complete profile",
  "Ім'я:": "First name:",
  "Ім'я": "First name",
  "Прізвище:": "Last name:",
  "Прізвище": "Last name",
  "Дата народження:": "Date of birth:",
  "Дата народження": "Date of birth",
  "Фото профілю:": "Profile photo:",
  "Фото профілю": "Profile photo",
  "Оберіть вид спорту:": "Choose sport:",
  "Оберіть вид спорту": "Choose sport",
  "Назва організації:": "Organization name:",
  "Назва організації": "Organization name",
  "Тип організації:": "Organization type:",
  "Тип організації": "Organization type",
  "Країна:": "Country:",
  "Країна": "Country",
  "Фото організації:": "Organization photo:",
  "Фото організації": "Organization photo",
  "Опис:": "Description:",
  "Опис": "Description",
  "Завершити": "Finish",
  "Збереження...": "Saving...",
  "Перевірка...": "Checking...",
  "Реєстрація...": "Registering...",
  "Будь ласка, заповніть всі поля!": "Please fill in all fields!",
  "Введіть код підтвердження": "Enter the confirmation code",

  // tournaments
  "SportHive · Match Center": "SportHive · Match Center",
  "Переглядай заходи, матчі, статуси, таблиці та сітки в одному місці.": "View events, matches, statuses, tables and brackets in one place.",
  "Види спорту": "Sports",
  "Усі види": "All sports",
  "Усі види спорту": "All sports",
  "Усі": "All",
  "Система": "System",
  "Усі системи": "All systems",
  "Заходів": "Events",
  "Заходи": "Events",
  "Захід": "Event",
  "Матчі": "Matches",
  "Матч": "Match",
  "Live": "Live",
  "Очікують": "Upcoming",
  "Очікується": "Upcoming",
  "Очікуються / Live": "Upcoming / Live",
  "Завершені": "Finished",
  "Завершено": "Finished",
  "Завершення матчів": "Match completion",
  "Live зараз": "Live now",
  "Обери захід": "Choose an event",
  "Тут буде детальна інформація, матчі, таблиця та сітка.": "Detailed information, matches, table and bracket will appear here.",
  "Таблиця": "Table",
  "Сітка": "Bracket",
  "Раунд": "Round",
  "Група": "Group",
  "Показати всі матчі": "Show all matches",
  "Матч завершений. Доступний перегляд результату.": "Match finished. Result view is available.",
  "Дата/час не вказані": "Date/time not specified",
  "Суддя:": "Judge:",
  "Score:": "Score:",
  "Переможець": "Winner",
  "Рахунок": "Score",
  "Таблиця зʼявиться після завершення хоча б одного матчу з рахунком або переможцем": "The table will appear after at least one match is finished with a score or winner.",
  "Сітка ще не сформована": "Bracket is not generated yet",

  // statistics
  "SportHive Analytics": "SportHive Analytics",
  "Глобальна статистика": "Global statistics",
  "Статистика моїх організацій": "My organizations statistics",
  "Лідери, сезони, країни, спорт, системи відбору, організації, команди, судді та спортсмени.": "Leaders, seasons, countries, sports, selection systems, organizations, teams, judges and athletes.",
  "Сезон": "Season",
  "Усі сезони": "All seasons",
  "Усі країни": "All countries",
  "Сортування": "Sorting",
  "Очки": "Points",
  "Перемоги": "Wins",
  "Різниця": "Difference",
  "Назва": "Name",
  "Матчів": "Matches",
  "Країн": "Countries",
  "Win Rate": "Win Rate",
  "Вся статистика": "Full statistics",
  "Аналітика сезону": "Season analytics",
  "Топ лідерів": "Top leaders",
  "Недостатньо завершених матчів": "Not enough finished matches",

  // organization profile
  "SportHive · Організація": "SportHive · Organization",
  "Про організацію": "About organization",
  "+ Створити команду": "+ Create team",
  "Керувати складом": "Manage roster",
  "Додати учасника через запрошення": "Add member by invitation",
  "Додати учасника в організацію": "Add member to organization",
  "Запрошення прийде користувачу на пошту. Для прямого додавання/видалення натисни “Керувати складом”.": "The invitation will be sent to the user's email. For direct add/remove use “Manage roster”.",
  "Запрошення прийде користувачу на пошту. Після прийняття він зʼявиться у складі організації.": "The invitation will be sent to the user's email. After acceptance, the user will appear in the organization roster.",
  "Кого додати": "Who to add",
  "Тренера": "Trainer",
  "Суддю": "Judge",
  "Спортсмена": "Athlete",
  "Пошук і вибір": "Search and select",
  "Надіслати запрошення": "Send invitation",
  "Користувача ще не вибрано": "No user selected yet",
  "Наші команди": "Our teams",
  "Останні події": "Recent events",
  "Запрошення": "Invitations",
  "Склад організації": "Organization roster",
  "Керування складом організації": "Organization roster management",

  // team page
  "SportHive · команда": "SportHive · team",
  "КОМАНДА": "TEAM",
  "Матчі команди": "Team matches",
  "Склад команди": "Team roster",
  "Назва команди": "Team name",
  "Вид спорту": "Sport",
  "Логін тренера": "Trainer login",
  "Логін спортсмена": "Athlete login",
  "Статус спортсмена": "Athlete status",
  "Додати / оновити": "Add / update",
  "Видалити спортсмена": "Remove athlete",
  "Видалити команду": "Delete team",

  // statuses
  "Finished": "Finished",
  "Upcoming": "Upcoming",
  "Active": "Active",
  "Reserve": "Reserve",
  "Captain": "Captain",
  "Main": "Main",
  "Pending": "Pending",
  "Accepted": "Accepted",
  "Rejected": "Rejected",

  // placeholders
  "Пошук заходу, команди, спортсмена...": "Search event, team, athlete...",
  "Введи імʼя, email або login...": "Enter name, email or login...",
  "athlete_login": "athlete_login",
  "trainer_login": "trainer_login",
  "user_login": "user_login",
  "Клуб / Школа / Федерація": "Club / School / Federation",

  // countries/sports
  "Україна": "Ukraine",
  "Польща": "Poland",
  "Румунія": "Romania",
  "Молдова": "Moldova",
  "Німеччина": "Germany",
  "Франція": "France",
  "Італія": "Italy",
  "Іспанія": "Spain",
  "Велика Британія": "United Kingdom",
  "США": "United States",
  "Канада": "Canada",
  "Інша країна": "Other country",
  "Шахи": "Chess",
  "Бокс": "Boxing",
  "Футбол": "Football",
  "Баскетбол": "Basketball",
  "Волейбол": "Volleyball",
  "Теніс": "Tennis",
  "Настільний теніс": "Table tennis",
  "Бадмінтон": "Badminton",
  "Хокей": "Hockey",
  "Боротьба": "Wrestling",
};

const UK: Record<string, string> = {};
const REVERSE_EN: Record<string, string> = {};

for (const [uk, en] of Object.entries(EN)) {
  UK[uk] = uk;
  REVERSE_EN[en] = uk;
}

const MUTATION_DELAY_MS = 40;
let observer: MutationObserver | null = null;
let observerTimer: number | undefined;
let isApplying = false;

export function getLanguage(): AppLanguage {
  const value = localStorage.getItem(STORAGE_KEY);

  return value === "en" ? "en" : "uk";
}

export function setLanguage(language: AppLanguage): void {
  localStorage.setItem(STORAGE_KEY, language);
  document.documentElement.lang = language === "en" ? "en" : "uk";
  document.body.dataset.lang = language;
  applyI18n(document.body);
  updateLanguageSwitcherState();
}

export function toggleLanguage(): void {
  setLanguage(getLanguage() === "uk" ? "en" : "uk");
}

export function t(ukText: string, language: AppLanguage = getLanguage()): string {
  if (language === "uk") return ukText;

  return EN[ukText] || ukText;
}

export function initI18n(): void {
  const language = getLanguage();

  document.documentElement.lang = language === "en" ? "en" : "uk";
  document.body.dataset.lang = language;

  applyI18n(document.body);
  startObserver();
}

export function renderLanguageSwitcher(): string {
  const language = getLanguage();

  return `
    <div class="lang-switcher" aria-label="Language switcher">
      <button class="lang-btn ${language === "uk" ? "active" : ""}" data-lang="uk" type="button">UA</button>
      <button class="lang-btn ${language === "en" ? "active" : ""}" data-lang="en" type="button">EN</button>
    </div>
  `;
}

export function bindLanguageSwitcher(onChange?: () => void): void {
  document.querySelectorAll<HTMLButtonElement>("[data-lang]").forEach(button => {
    button.addEventListener("click", () => {
      const lang = button.dataset.lang === "en" ? "en" : "uk";
      setLanguage(lang);
      onChange?.();
    });
  });

  updateLanguageSwitcherState();
}

export function applyI18n(root: ParentNode = document.body): void {
  if (!root) return;

  isApplying = true;

  try {
    const language = getLanguage();

    translateTextNodes(root, language);
    translateElementAttributes(root, language);
    updateLanguageSwitcherState();
  } finally {
    window.setTimeout(() => {
      isApplying = false;
    }, 0);
  }
}

function startObserver(): void {
  if (observer) return;

  observer = new MutationObserver(() => {
    if (isApplying) return;

    window.clearTimeout(observerTimer);

    observerTimer = window.setTimeout(() => {
      applyI18n(document.body);
    }, MUTATION_DELAY_MS);
  });

  observer.observe(document.body, {
    childList: true,
    subtree: true,
    characterData: true,
  });
}

function updateLanguageSwitcherState(): void {
  const language = getLanguage();

  document.querySelectorAll<HTMLButtonElement>(".lang-btn").forEach(button => {
    button.classList.toggle("active", button.dataset.lang === language);
  });
}

function translateTextNodes(root: ParentNode, language: AppLanguage): void {
  const walker = document.createTreeWalker(
    root,
    NodeFilter.SHOW_TEXT,
    {
      acceptNode(node) {
        const parent = node.parentElement;

        if (!parent) return NodeFilter.FILTER_REJECT;

        if (shouldSkip(parent)) return NodeFilter.FILTER_REJECT;

        if (!node.textContent || !node.textContent.trim()) {
          return NodeFilter.FILTER_REJECT;
        }

        return NodeFilter.FILTER_ACCEPT;
      },
    }
  );

  const nodes: Text[] = [];

  while (walker.nextNode()) {
    nodes.push(walker.currentNode as Text);
  }

  for (const node of nodes) {
    const original = node.textContent || "";
    const translated = translateText(original, language);

    if (translated !== original) {
      node.textContent = translated;
    }
  }
}

function translateElementAttributes(root: ParentNode, language: AppLanguage): void {
  const elements = root instanceof Element
    ? [root, ...Array.from(root.querySelectorAll<HTMLElement>("*"))]
    : Array.from(root.querySelectorAll<HTMLElement>("*"));

  for (const element of elements) {
    if (shouldSkip(element)) continue;

    translateAttr(element, "placeholder", language);
    translateAttr(element, "title", language);
    translateAttr(element, "aria-label", language);
    translateAttr(element, "alt", language);

    if (element instanceof HTMLInputElement && ["button", "submit", "reset"].includes(element.type)) {
      const value = element.value;

      if (value) {
        const translated = translateText(value, language);

        if (translated !== value) element.value = translated;
      }
    }
  }
}

function translateAttr(element: Element, attr: string, language: AppLanguage): void {
  const value = element.getAttribute(attr);

  if (!value) return;

  const translated = translateText(value, language);

  if (translated !== value) {
    element.setAttribute(attr, translated);
  }
}

function shouldSkip(element: Element): boolean {
  const tag = element.tagName.toLowerCase();

  if (["script", "style", "code", "pre", "textarea"].includes(tag)) return true;
  if (element.closest("[data-i18n-ignore]")) return true;

  return false;
}

function translateText(text: string, language: AppLanguage): string {
  const leading = text.match(/^\s*/)?.[0] || "";
  const trailing = text.match(/\s*$/)?.[0] || "";
  const core = text.trim();

  if (!core) return text;

  const canonical = toCanonicalUk(core);

  if (language === "uk") {
    const translated = replaceKnownParts(canonical, "uk");
    return `${leading}${translated}${trailing}`;
  }

  const exact = EN[canonical];

  if (exact) {
    return `${leading}${exact}${trailing}`;
  }

  const translated = replaceKnownParts(canonical, "en");
  return `${leading}${translated}${trailing}`;
}

function toCanonicalUk(text: string): string {
  if (UK[text]) return text;
  if (REVERSE_EN[text]) return REVERSE_EN[text];

  let value = text;

  // Convert English phrases back to Ukrainian before applying a new target language.
  const entries = Object.entries(REVERSE_EN).sort((a, b) => b[0].length - a[0].length);

  for (const [en, uk] of entries) {
    if (value.includes(en)) {
      value = value.split(en).join(uk);
    }
  }

  return value;
}

function replaceKnownParts(text: string, language: AppLanguage): string {
  const sourceMap = language === "en" ? EN : UK;
  const reverseMap = language === "uk" ? REVERSE_EN : {};
  let value = text;

  if (language === "uk") {
    const entries = Object.entries(reverseMap).sort((a, b) => b[0].length - a[0].length);

    for (const [en, uk] of entries) {
      if (value.includes(en)) {
        value = value.split(en).join(uk);
      }
    }

    return value;
  }

  const entries = Object.entries(sourceMap).sort((a, b) => b[0].length - a[0].length);

  for (const [uk, en] of entries) {
    if (value.includes(uk)) {
      value = value.split(uk).join(en);
    }
  }

  return value;
}
