namespace Jinget.Blazor.Attributes.Input
{
    public abstract class JingetInputElement : JingetFormElement
    {
        internal InputType GetInputType()
        {
            return this switch
            {
                JingetTextBox => InputType.Text,
                JingetTextArea => InputType.Text,
                JingetPasswordBox => InputType.Password,
                JingetHiddenBox => InputType.Hidden,
                JingetNumberBox => InputType.Number,
                JingetEmailBox => InputType.Email,
                JingetUrlBox => InputType.Url,
                JingetTelephoneBox => InputType.Telephone,
                JingetColorBox => InputType.Color,
                JingetTimeBox => InputType.Time,
                JingetMonthBox => InputType.Month,
                JingetDateBox => InputType.Date,
                JingetDateTimeLocalBox => InputType.DateTimeLocal,
                JingetWeekBox => InputType.Week,
                _ => InputType.Text,
            };
        }
    }
}
