import {JingetDateTimePicker} from "../jinget.dp";

export class JingetDpDataMapper {
    private static map: Map<string, JingetDateTimePicker> = new Map();

    public static set(key: string, instance: JingetDateTimePicker): void {
        if (!this.map.has(key)) {
            this.map.set(key, instance);
            return;
        }
        this.map.set(key, instance);
    }

    public static get = (key: string): JingetDateTimePicker | null => this.map.get(key) || null;

    public static getAll = (): JingetDateTimePicker[] => Array.from(this.map, ([_name, value]) => value);

    public static remove(key: string): void {
        if (!this.map.has(key)) {
            return;
        }
        this.map.delete(key);
    }
}



