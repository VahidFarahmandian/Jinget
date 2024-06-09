namespace Jinget.Blazor.Attributes.Input
{
    public abstract class JingetBaseInputElement : JingetFormElement
    {
        internal InputType GetInputType()
        {
            return this switch
            {
                JingetTextBoxElement => InputType.Text,
                JingetTextAreaElement => InputType.Text,
                JingetPasswordBoxElement => InputType.Password,
                JingetHiddenBoxElement => InputType.Hidden,
                JingetNumberBoxElement => InputType.Number,
                JingetEmailBoxElement => InputType.Email,
                JingetUrlBoxElement => InputType.Url,
                JingetTelephoneBoxElement => InputType.Telephone,
                JingetColorBoxElement => InputType.Color,
                JingetTimeBoxElement => InputType.Time,
                JingetMonthBoxElement => InputType.Month,
                JingetDateBoxElement => InputType.Date,
                JingetDateTimeLocalBoxElement => InputType.DateTimeLocal,
                JingetWeekBoxElement => InputType.Week,
                _ => InputType.Text,
            };
        }
    }
}
