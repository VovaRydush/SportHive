export type PickerEntityType =
  | "athlete"
  | "team"
  | "trainer"
  | "judge"
  | "organization"
  | "user";

export type PickerEntity = {
  id: string;
  title: string;
  subtitle?: string;
  photo?: string;
  type: PickerEntityType;
  raw?: unknown;
  manual?: boolean;
};

export type PickerConfig = {
  entityType: PickerEntityType;
  multiple?: boolean;
  placeholder?: string;
  minLength?: number;
  selected?: PickerEntity[];
  onChange: (items: PickerEntity[]) => void;
};
