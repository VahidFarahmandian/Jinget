namespace Jinget.Blazor.Attributes.Input;

public abstract class JingetBaseInputElement : JingetFormElement
{
    internal Enums.InputType GetInputType() => this switch
    {
        JingetTextBoxElement => Enums.InputType.Text,
        JingetTextAreaElement => Enums.InputType.Text,
        JingetPasswordBoxElement => Enums.InputType.Password,
        JingetHiddenBoxElement => Enums.InputType.Hidden,
        JingetNumberBoxElement => Enums.InputType.Number,
        JingetEmailBoxElement => Enums.InputType.Email,
        JingetUrlBoxElement => Enums.InputType.Url,
        JingetTelephoneBoxElement => Enums.InputType.Telephone,
        JingetColorBoxElement => Enums.InputType.Color,
        JingetTimeBoxElement => Enums.InputType.Time,
        JingetMonthBoxElement => Enums.InputType.Month,
        JingetDateBoxElement => Enums.InputType.Date,
        JingetDateTimeLocalBoxElement => Enums.InputType.DateTimeLocal,
        JingetWeekBoxElement => Enums.InputType.Week,
        _ => Enums.InputType.Text,
    };
}
