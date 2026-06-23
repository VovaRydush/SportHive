export type CountryOption = {
  code: string;
  name: string;
};

export const COUNTRY_OPTIONS: CountryOption[] = [
  { code: "Ukraine", name: "Україна" },
  { code: "Poland", name: "Польща" },
  { code: "Romania", name: "Румунія" },
  { code: "Moldova", name: "Молдова" },
  { code: "Germany", name: "Німеччина" },
  { code: "France", name: "Франція" },
  { code: "Italy", name: "Італія" },
  { code: "Spain", name: "Іспанія" },
  { code: "Portugal", name: "Португалія" },
  { code: "United Kingdom", name: "Велика Британія" },
  { code: "United States", name: "США" },
  { code: "Canada", name: "Канада" },
  { code: "Turkey", name: "Туреччина" },
  { code: "Georgia", name: "Грузія" },
  { code: "Lithuania", name: "Литва" },
  { code: "Latvia", name: "Латвія" },
  { code: "Estonia", name: "Естонія" },
  { code: "Czech Republic", name: "Чехія" },
  { code: "Slovakia", name: "Словаччина" },
  { code: "Hungary", name: "Угорщина" },
  { code: "Other", name: "Інша країна" }
];

export function renderCountryOptions(selected?: string) {
  return COUNTRY_OPTIONS.map(country => `
    <option value="${country.code}" ${selected === country.code ? "selected" : ""}>${country.name}</option>
  `).join("");
}
