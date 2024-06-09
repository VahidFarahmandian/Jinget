using Jinget.Blazor.Attributes.Input;
using Jinget.Blazor.Attributes;
using Jinget.Blazor.Models;
using Jinget.Blazor.Services.Contracts;
using MudBlazor;

namespace Jinget.Blazor.Test
{
    public record SampleModel
    {
        public SampleModel()
        {

        }
        //ITokenStorageService tokenService;
        //ILocalStorageService localStorage;
        //public SampleModel(IServiceProvider serviceProvider)
        //{
        //    tokenService = serviceProvider.GetRequiredService<ITokenStorageService>();
        //    localStorage = serviceProvider.GetRequiredService<ILocalStorageService>();
        //}
        [JingetTextBoxElement(DisplayName = "نام", HelperText = "نام خود را منطبق با اطلاعات کارت ملی وارد نمایید", Order = 1)]
        public string? Name { get; set; }

        [JingetTextBoxElement(DisplayName = "نام خانوادگی", HelperText = "نام خانوادگی خود را منطبق با اطلاعات کارت ملی وارد نمایید", Order = 2)]
        public string? LastName { get; set; }

        [JingetPasswordBoxElement(DisplayName = "رمز عبور", Order = 3)]
        public string? Password { get; init; }

        [JingetEmailBoxElement(DisplayName = "پست الکترونیکی", Order = 4)]
        public string? EMail { get; init; }

        [Attributes.Picker.JingetDatePickerElement(DisplayName = "تاریخ تولد", Culture = "fa-IR", Order = 5)]
        public string? DoB { get; init; }

        [Attributes.Picker.JingetDateRangePickerElement(DisplayName = "بازه زمانی سفر", Culture = "fa-IR", Order = 6)]
        public DateRange? TravelDate { get; init; }

        [JingetLabelElement(DisplayName = "امتیاز اکتسابی", HasLabel = false)]
        public int Score { get; set; } = 1850;

        [JingetTextAreaElement(DisplayName = "اطلاعات بیشتر", Rows = 3)]
        public string? Description { get; set; }

        [JingetNumberBoxElement(DisplayName = "سن", Order = 7)]
        public int Age { get; set; }

        [JingetDropDownListElement(DisplayName = "وضعیت2", Id = "cmb2",/* BindingFunction = nameof(GetStatusAsync),*/ DefaultText = "---انتخاب کنید---",
        Order = 8/*, GetTokenBeforeBinding = true*/)]
        public int? Status2 { get; set; }

        [JingetDropDownListElement(DisplayName = "وضعیت", Id = "cmbSearch",
        BindingFunction = nameof(GetStatusAsync), PreBindingFunction = nameof(PreBinding), PostBindingFunction = nameof(PostBinding),
        Searchable = true, DefaultText = "---انتخاب کنید---", HasLabel = true, LabelCssClass = "overlayed-label", Order = 9/*, GetTokenBeforeBinding = true*/)]
        public int? Status { get; set; }
        //public async Task<string> PreBinding(string? token) => await Task.FromResult("This is pre binding");
        public async Task<string> PreBinding() => await Task.FromResult("This is pre binding");
        //public async Task<string> PostBinding(string? token, object? preBindingResult, object? data) => await Task.FromResult("This is post binding");
        public async Task<string> PostBinding(object? preBindingResult, object? data) => await Task.FromResult("This is post binding");
        //public async Task<List<JingetDropDownItemModel>> GetStatusAsync(string token, object? preBindingResult)
        public async Task<List<JingetDropDownItemModel>> GetStatusAsync(object? preBindingResult)
        => await new JingetDropDownListElement().BindAsync(async () =>
        {
            var t = preBindingResult;
            return await Task.FromResult(new List<StatusModel> {
                new() { Code = 1, Title = "فعال" },
                new() { Code = 2, Title = "غیرفعال" },
                new() { Code = 3, Title = "نامشخص" }
            });
        });

       public class StatusModel : BaseTypeModel
        {

        }
    }
}
