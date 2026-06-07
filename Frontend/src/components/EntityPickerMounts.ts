import { AdaptiveEntityPicker } from "./AdaptiveEntityPicker";
import type { PickerEntity } from "../api/entityPickerTypes";

/*
  Use these helpers anywhere you currently have plain text search for:
  athletes, teams, trainers, judges, organizations, users.

  Example HTML:
  <div id="athlete-picker"></div>
  <input type="hidden" id="selected-athletes-json" />
*/

export function mountAthletePicker(containerId: string, onChange: (items: PickerEntity[]) => void) {
  return mountPicker(containerId, "athlete", true, "Знайти спортсмена за ПІБ або логіном...", onChange);
}

export function mountTeamPicker(containerId: string, onChange: (items: PickerEntity[]) => void) {
  return mountPicker(containerId, "team", true, "Знайти команду...", onChange);
}

export function mountTrainerPicker(containerId: string, onChange: (items: PickerEntity[]) => void) {
  return mountPicker(containerId, "trainer", false, "Знайти тренера...", onChange);
}

export function mountJudgePicker(containerId: string, onChange: (items: PickerEntity[]) => void) {
  return mountPicker(containerId, "judge", true, "Знайти суддю...", onChange);
}

export function mountOrganizationPicker(containerId: string, onChange: (items: PickerEntity[]) => void) {
  return mountPicker(containerId, "organization", false, "Знайти організацію...", onChange);
}

export function mountUserPicker(containerId: string, onChange: (items: PickerEntity[]) => void) {
  return mountPicker(containerId, "user", true, "Знайти користувача...", onChange);
}

function mountPicker(
  containerId: string,
  entityType: "athlete" | "team" | "trainer" | "judge" | "organization" | "user",
  multiple: boolean,
  placeholder: string,
  onChange: (items: PickerEntity[]) => void
) {
  const root = document.getElementById(containerId);

  if (!root) {
    throw new Error(`Picker container '${containerId}' not found`);
  }

  const picker = new AdaptiveEntityPicker(root, {
    entityType,
    multiple,
    placeholder,
    onChange,
  });

  picker.render();

  return picker;
}
