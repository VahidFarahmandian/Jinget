# Jinget Blazor
Blazor components optimised for RTL languaged such as Farsi, Arabic etc.

**Components list:**
- [x] **Captcha**
- [x] **Date Picker**
- [x] **Date Range Picker**
- [x] **Message Box**
- [x] **Modal**
- [x] **List**
- [x] **Table**
- [x] **Dynamic Form**
- [x] **JingetDropDownList** 
- [x] **JingetDropDownListTree** 
- [x] **Input** 
- [x] **JsonVisualizer** 
- [x] **Gauge** 

**Services list:**
- [x] **LocalStorage**
- [x] **LocalStorage**
- [x] **SessionStorage**


## How to Use:

Download the package from NuGet using Package Manager:
`Install-Package Jinget.Blazor`
You can also use other methods supported by NuGet. Check [Here](https://www.nuget.org/packages/Jinget.Blazor "Here") for more information.


## Add Services:

Register the required services in your `Program.cs` file:

```
builder.Services.AddJingetBlazor();
```

If you need to use `TokenAuthenticationStateProvider` as a provider for handling authentication state(for example in your ASP.NET Core Blazor project), 
then you need to pass `addAuthenticationStateProvider=true` to `AddJingetBlazor`.
If you need to use token storage services for storing/retrieving your tokens, then you need to pass `tokenConfigModel` and `addTokenRelatedServices=true` to `AddJingetBlazor`.
If you do not need to use token storage services, but you want to register `TokenConfigModel` then you need to pass `tokenConfigModel` and `addTokenRelatedServices=false` to `AddJingetBlazor`.
If you have no plan to use `Jinget.Blazor` components and you just want to use its services, then you can pass `addComponents=false` to `AddJingetBlazor` method.


## How to Use Methods:

***Login and Logout user:***

If you want to authenticate the user and manage user's state via `TokenAuthenticationStateProvider` then you can inject `UserService`
to your login form and authenticate the user:

```
@attribute [AllowAnonymous]
@layout LoginLayout
@page "/login"
@inject NavigationManager NavigationManagerService;
@inject UserService userService;
@inherits BaseComponent
@{
    base.BuildRenderTree(__builder);
}
<input type="text" id="username" @bind-value="@UserName" />
<input type="password" id="password" @bind-value="@Password" />
<button @onclick="Authenticate">ورود</button>

@code {

    string? UserName { get; set; }
    string? Password { get; set; }

    async Task Authenticate()
    {
        var result = await userService.LoginAsync(UserName, Password);

        if (result)
        {
                NavigationManagerService.NavigateTo("/", true);
        }
        else
        {
            await ShowErrorAsync("Username or password is incorrect");
        }
    }
}

```

`await userService.LoginAsync` method will call the `LoginAsync` defined in `IAuthenticationService` interface. 
You can implement `IAuthenticationService` interface and write your own logic to authenticate the user.
Similiarly in order to logout the user you can call `await userService.LogoutAsync()`.


## How to Use Components:

***References:***

Add reference to `jinget.core.css` in your `_Host.razor` or `App.razor` files.

```
<link href="_content/Jinget.Blazor/css/jinget.core.css" rel="stylesheet" />
```

Install `MudBlazor` and add reference to `MudBlazor.min.js` in your `_Host.razor` or `App.razor` files.

```
<script src="_content/MudBlazor/MudBlazor.min.js"></script>
```

Add reference to `jinget.core.js` in your `_Host.razor` or `App.razor` files.

```
<script src="_content/Jinget.Blazor/js/jinget.core.js" id="jinget"></script>
```

**Jinget Captcha component**

Add the `JingetCaptcha` to your page and start using it;-)

```
<JingetCaptcha Width="200"
               Height="50"
               CharCount="5"
               CharSetType="CharSetTypeOptions.EnglishCharsPlusDigit"
               ContentCaseSensitivity="CharSetCaseSensitivityOptions.IgnoreCase"
               CaseSesitiveComparison=false>
</JingetCaptcha>
```
***Parameters:***

`Width`: Captcha image box width. Default is 170.

`Height`: Captcha image box height. Default is 40.

`CharCount`: Total number of characters to be rendered in image. Default value is 5. The value should be greater than 0 otherwise it will be replaced by 5

`CharSetType`: Set the character set used for rendering the image. Default is DigitOnly. Supported values are: DigitOnly, EnglishCharsOnly, FarsiCharsOnly, EnglishCharsPlusDigit, FarsiCharsPlusDigit, EnglishCharsPlusFarsiChars, BlendAll, Custom

`ContentCaseSensitivity`: Set the character set case sensitivity used for rendering the image. This property is only applied to the English characters. Default value is IgnoreCase. Supported values are: UpperCaseOnly, LowerCaseOnly, IgnoreCase

`CaseSesitiveComparison`: Check if the input captcha text validation should be done in case sensitive or case insensitive manner.

`CustomCharSet`: If ContentCharsType is set to Custom then characters provided by CustomCharSet will be used to render the image.

`IsRtlCharSet`: If RTL language's character set is being used(such as Farsi, Arabic, etc), this property should be set to true.

`FontFamilies`: Sets the Font families used for drawing the text inside the image. Default Values are: Arial, Tahoma and Times New Roman

***Callbacks:***

`CaptchaChanged`: Fires a callback whenever the captcha image is changed

***Methods:***

`IsValid`: Checks if the given input text is equal to the image's text

`GetNewCaptchaAsync`: Draw a new captcha image

------------

**Jinget DatePicker component**

Add the `JingetDatePicker` to your page and start using it;-)

```
<JingetDatePicker 
    Value=selectedDate_1
    DateChanged="OnDateChanged_1">
</JingetDatePicker>
...
@code {
    DateTime? selectedDate_1;
    void OnDateChanged_1(DateTime? selectedDate) => selectedDate_1 = selectedDate;
}
```

***Parameters:***

`Value`: Defines the picker's value

`Culture`: Defines the date picker culture. For Jalali calendar you can set this to fa-IR, for Hijri calendar you can set this to arabic cultures such as ar-SA, for gregorian calendar you can set this to an appropriate calendar such as en-US. Other cultures are also supported such as chinese: zh-Hans

`Disabled`: If set to true, then the calendar will be disabled.

`ReadOnly`: If set to true, then the calendar will be rendered in readonly mode.

`Editable`: If set to true, user can select date using keyboard.(Default is true. for fa-IR and ar-SA cultures this property is set to false)

`Clearable`: If set ti true, clear button will be shown for clearing the selected date.

`MinDate`: Defines the minimum acceptable date.

`MaxDate`: Defines the maximum acceptable date.

`Required`: Defines whether the input is required or optional.

`RequiredError`: Error message used to show when the input is required and remained empty or unselected.

`EnglishNumber`: Defines whether the numbers in calendar should be rendred as English numbers or not.

`Label`: Label for the component.

`DisabledDateFunc`: Defines a Func delegate to disable the specific date(s)

`CustomDateStyleFunc`: Defines a Func delegate to use custom styles for decorating cusotm dates

***Callbacks:***

`DateChanged`: Fires a callback whenever the selected date is changed

`OnRendered`: whenever the component rendered on the page.

------------

**Jinget DateRangePicker component**

Add the `JingetDateRangePicker` to your page and start using it;-)

```
<JingetDateRangePicker 
    Value=selectedDateRange_1.DateRange
    DateRangeChanged="OnDateRangeChanged_1">
</JingetDateRangePicker>
...
@code {
    SelectedDateRangeModel selectedDateRange_1 = new();
    void OnDateRangeChanged_1(SelectedDateRangeModel selectedDateRange) => selectedDateRange_1 = selectedDateRange;
}
```

***Parameters:***

`Value`: Defines the picker's value

`Culture`: Defines the date picker culture. For Jalali calendar you can set this to fa-IR, for Hijri calendar you can set this to arabic cultures such as ar-SA, for gregorian calendar you can set this to an appropriate calendar such as en-US. Other cultures are also supported such as chinese: zh-Hans

`Disabled`: If set to true, then the calendar will be disabled.

`ReadOnly`: If set to true, then the calendar will be rendered in readonly mode.

`Editable`: If set to true, user can select date using keyboard.(Default is true. for fa-IR and ar-SA cultures this property is set to false)

`Clearable`: If set ti true, clear button will be shown for clearing the selected date.

`MinDate`: Defines the minimum acceptable date.

`MaxDate`: Defines the maximum acceptable date.

`Required`: Defines whether the input is required or optional.

`RequiredError`: Error message used to show when the input is required and remained empty or unselected.

`EnglishNumber`: Defines whether the numbers in calendar should be rendred as English numbers or not.

`Label`: Label for the component.

`DisabledDateFunc`: Defines a Func delegate to disable the specific date(s)

`CustomDateStyleFunc`: Defines a Func delegate to use custom styles for decorating cusotm dates


***Callbacks:***

`DateRangeChanged`: Fires a callback whenever the selected date range is changed.

`OnRendered`: whenever the component rendered on the page.

------------

**Jinget Modal component**

Add the `JingetModal` to your page and start using it;-)

```
<JingetModal @ref="@modal1"
             Title="Sample Header"
             ShowFooter=true
             ShowHeader=true
             Rtl=false
             CloseButtonText="Close">
    <ChildContent>
        <div class="alert alert-secondary" role="alert">
            This is a sample modal
        </div>
    </ChildContent>
</JingetModal>
...
@code {
    JingetModal? modal1 { get; set; }
    async Task ShowModal1() => await modal1?.OpenAsync();
}
```

***Parameters:***

`ShowHeader`: Defines whether to show the header or not.

`ShowFooter`: Defines whether to show the footer or not.

`Title`: Dfines the modal title.

`ChildContent`: Defines the content which should be rendered as component's child.

`FooterContent`: Defines the content which should be rendered as component's footer.

`Rtl`: If set to true, the the modal content will be rendred Right-to-Left.

`CloseButtonText`: Defines the close button text.

`CloseButtonCss`: Css class used for close button placed in modal's footer.

`ShowCloseOnHeader`: if set true, then close button(X) will be shown on modal's header

***Callbacks:***

`OnOpen`: Fires a callback whenever the modal is being opened.

`OnClose`: Fires a callback whenever the modal is being closed.

------------

**Jinget MessageBox component**

Add the `JingetMessageBox` to your page and start using it;-)

```
<JingetMessageBox @ref=@messageBox OnOpen=Opened>
</JingetMessageBox>
...
@code {
    void Opened() => messageBox.CloseButtonText = messageBox.Rtl ? "بستن" : "Close";
}
```

***Parameters:***

`ShowVerboseFuncAsync`: Defines a Func delegate to show the verbose content

`Rtl`: If set to true, the the modal content will be rendred Right-to-Left.

`CloseButtonText`: Defines the close button text.

***Callbacks:(shared with JingetMessageBox Component)***

`OnOpen`: Fires a callback whenever the modal is being opened.

`OnClose`: Fires a callback whenever the modal is being closed.

***Methods:***

`OpenAsync`: Opens the message box.

`CloseAsync`: Closes the message box.

`ShowErrorAsync`: Opens the message box using Error theme.

`ShowWarningAsync`: Opens the message box using Warning theme.

`ShowInfoAsync`: Opens the message box using Info theme.

------------

**Jinget List component**

Add the `JingetList` to your page and start using it;-)

```
<JingetList 
    Model=@Model 
    Title="Sample List" 
    HeaderCssClass="list-group-item-action active">
</JingetList>
...
@code {
    IList<ListItem> Model = new List<ListItem>
    {
        new ListItem("Item 1","list-group-item-info"),
        new ListItem("Item 2","list-group-item-secondary"),
        new ListItem("Item 3"),
    };
}
```

***Parameters:***

`Model`: Model used to bind to the component.

`Title`: If `ShowTitle` is set to true, then `Title` will be rendered as list's header text.

`ShowTitle`: Defines whether to show the `Title` or not.

`HeaderCssClass`: Defines a custom css class used to decorate the list's header.

------------

**Jinget Table component**

Add the `JingetTable` to your page and start using it;-)

```
<JingetTable Id="tblSample" Model=@Model
             OnDataBind="BindData"
             OnRowSelected="@((SampleDataModel e)=>RowSelected(e))"
             FirstPageText="<<"
             PrevPageText="<"
             LastPageText=">>"
             NextPageText=">"
             RowsPerPageString="Per page:"
             AllItemsText="All"
             AscendingSortText="Ascending"
             DescendingSortText="Descending"
             PageInfoTextFormat="{first_item}-{last_item} FROM {all_items}"
             SearchBadgeTextFormat="FILTERED BY:"
             SearchPlaceHolderText="Search"
             SelectedRowBadgeTextFormat="SELECTED ROW:"
             SortBadgeTextFormat="SORTED BY: {sort_col} {sort_dir}"
             IsRtl=false
             ShowBadgeBar=true
             PaginationPageSizeOptions=[5,10,20,30,40]>
    <NoRecordContent>No record found!</NoRecordContent>
    <PreActionContent>
        <MudButton Variant="Variant.Filled"
                   Color="Color.Success">
            <MudText>Pre Sample</MudText>
        </MudButton>
    </PreActionContent>
    <PostActionContent>
        <MudButton Variant="Variant.Filled"
                   Color="Color.Primary">
            <MudText>Post Sample</MudText>
        </MudButton>
    </PostActionContent>
</JingetTable>
...
@code {
    JingetMessageBox? messageBox;
    JingetTableModel<SampleDataModel>? Model;

    async Task RowSelected(SampleDataModel? e)
    {
        if (e != null)
        {
            await messageBox.ShowInfoAsync(
            e.Id.ToString(),
            $"{e.Name} {e.LastName}",
            System.Text.Json.JsonSerializer.Serialize(e), rtl: true);
        }
    }

    void BindData(JingetTableDataBindModel e)
    {
        var data = GetDataEng()
                .Where(x => string.IsNullOrWhiteSpace(e.SearchTerm) || x.Name.StartsWith(e.SearchTerm) || x.LastName.StartsWith(e.SearchTerm))
                .AsQueryable();
        var items = data
                    .OrderByDynamic(e.SortColumn, e.SortDirection)
                    .Skip((e.PageIndex - 1) * e.PageSize)
                    .Take(e.PageSize)
                    .ToList();
        ModelEng = new JingetTableModel<SampleDataModel>
            {
                Items = items,
                TotalItems = data.Count()
            };
        StateHasChanged();
    }

    List<SampleDataModel> GetData()
    {
        return new List<SampleDataModel>
            {
                new SampleDataModel(1,"Vahid","Farahmandian",34,true),
                new SampleDataModel(2,"Ali","Ahmadi",40,true),
                new SampleDataModel(3,"Mohsen","Nowroozi",18,true),
                new SampleDataModel(4,"Maryam","Ghane'ei",24,false),
                new SampleDataModel(5,"Sara","Hosseinzadeh",37,false),
                new SampleDataModel(6,"Amir","Rahmani",29,true),
                new SampleDataModel(7,"Seyed Rahman","Raoofi Asl",54,true),
                new SampleDataModel(8,"Saman","Sadeghi",41,true),
                new SampleDataModel(9,"Aboalfazl","Behnampour",19,true),
                new SampleDataModel(10,"Zhale","Alizadeh",38,false),
                new SampleDataModel(11,"Seyedreza","Aboalfathi",47,false),
                new SampleDataModel(12,"Mahtab","Asemani",26,true)
            };
    }

    [JingetTableElement]
    public class SampleDataModel
    {
        public SampleDataModel(int id, string name, string lastname, int age, bool isActive)
        {
            Id = id;
            Name = name;
            LastName = lastname;
            Age = age;
            IsActive = isActive;
        }

        [JingetTableMember(DisplayName = "#")]
        public int Id { get; set; }

        [JingetTableMember(DisplayName = "Name")]
        public string Name { get; set; }

        [JingetTableMember(DisplayName = "Lastname")]
        public string LastName { get; set; }

        [JingetTableMember(DisplayName = "Age", Sortable = true)]
        public int Age { get; set; }

        [JingetTableMember(DisplayName = "Status")]
        public bool IsActive { get; set; }
    }
}
```

***Parameters:***

`Model`: Model used to bind the table. Class defining the model should have `JingetTable` attributes. Also each property used to rendered as table's column should have `JingetTableMember` attribute. 

`IsRtl`: If set to true, the the modal content will be rendred Right-to-Left. Default is `true`

`NoRecordContent`: Dynamic content used to load inside table body, whenever table contains no data

`PreActionContentHeaderText`: header text used to display as pre action content column header.

`PreActionContent`: Dynamic content which is rendered before rendering the data row.

`PostActionContentHeaderText`: header text used to display as post action content column header.

`PostActionContent`: Dynamic content which is rendered after rendering the data row.

`ShowBadgeBar`: Defines whether to show to badge bar or not.

`ShowPagination`: Defines whether to show the pagination bar or not. default is true.

`FirstPageText`: First page button text in pagination bar. default is 'اولین'.

`PrevPageText`: Prev page button text in pagination bar. default is 'قبلی'.

`NextPageText`: Next page button text in pagination bar. default is 'بعدی'.

`LastPageText`: Last page button text in pagination bar. default is 'آخرین'.

`AllItemsText`: All items text inside the pagi size dropdownlist in pagination bar. default is 'همه'.

`PaginationPageSizeOptions`: Page size options inside the page size dropdownlist in pagination bar. default is [5,10,20,50,100].

`RowsPerPageText`: Page size label text in pagination bar. default is 'اندازه در هر صفحه:'.

`PageInfoTextFormat`: page info label text in pagination bar. default is '{first_item}-{last_item} از {all_items}'.

`RowIsSelectable`: Defines whether to select row on row click or not.

`ShowSelectedRowInBadgeBar`: Defines whether to show selected row in badge bar or not. default is 'false'.

`SelectedRowBadgeTextFormat`: Selected row label text in badge bar. default is 'سطر انتخابی:'.

`SelectedRowCss`: Css class used to style the selected row. default is 'table-info'.

`ShowSearchBar`: Defines whether to show the search bar or not. default is true.

`SearchPlaceHolderText`: Text used to bind as search input placeholder. default is 'جستجو'.

`SearchBadgeTextFormat`: Search term badge text format. default is: 'فیلتر شده براساس مقدار: {search_term}'.

`SearchBarContent`: Dynamic content used to load inside search bar.

`Sortable`: Defines whether columns in table are sortable or not. If columns are sortable then each sortable column should have Sortable=true in its attribute. default is 'true'

`SortBadgeTextFormat`: sort badge text format. default is: 'مرتب شده براساس: {sort_col} بصورت {sort_dir}'.

`AscendingSortText`: ascending sort badge text. default is 'صعودی'.

`DescendingSortText`: descending sort badge text. default is 'صعودی'.

***Events:***

`OnDataBind`: Event which is fired whenever the table is being binded.

`OnRowSelected`: Event which is fired whenever the user selects a row in table.

------------

**Jinget DynamicForm component**

Add the `JingetDynamicForm` to your page and start using it;-)

Note that if types used for dynamic form creation needs to access `IServiceProvider`, should inherit from `DynamicFormBaseModel`.

```
<JingetDynamicForm Model=@Model Rtl=true></JingetDynamicForm>
...
@code {
    public SampleModel Model { get; set; }
    protected override void OnInitialized() => Model = new();

    public class SampleModel: DynamicFormBaseModel
    {
        [JingetTextBoxElement(DisplayName = "Full Name", HelperText = "Please enter your full name", Order =1)]
        public string Name { get; set; }

        [JingetPasswordBoxElement(DisplayName = "Password", Order =2)]
        public string Password { get; init; }

        [JingetEmailBoxElement(DisplayName = "E-Mail",Order =3)]
        public string EMail { get; init; }

        [JingetDatePickerElement(DisplayName = "Date of Birth",Culture ="fa-IR", Order =4)]
        public string DoB { get; init; }

        [JingetDateRangePickerElement(DisplayName = "Travel Date range",Culture ="fa-IR", Order =5)]
        public DateRange TravelDate { get; init; }

        [JingetLabelElement(DisplayName = "Score", HasLabel = true)]
        public int Score { get; init; } = 1850;

        [JingetTextAreaElement(DisplayName = "More info", Rows =3)]
        public string Description { get; init; }

        [JingetNumberBoxElement(DisplayName = "Age", Order =7)]
        public int Age { get; set; }

        [JingetDropDownListElement(DisplayName = "Flight Status", BindingFunction = nameof(GetStatusAsync), 
        IsSearchable =true,DefaultText ="---Choose one---",HasLabel =true, LabelCssClass = "overlayed-label", Order =8)]
        public int? Status { get; init; }
        public async Task<List<JingetDropDownItemModel>> GetStatusAsync(object? preBindingResult)
            => await new JingetDropDownListElement().BindAsync<FlightStatusModel>(async () =>
            {
                return await Task.FromResult(new List<FlightStatusModel>
                {
                    new FlightStatusModel {Code= 1,Title= "Boarding" },
                    new FlightStatusModel {Code= 2,Title= "Arriving" }
                });
        });

        [JingetDropDownListTreeElement(DisplayName = "Geo", Id = "cmbTreeGeo", IsRtl = false,
            BindingFunction = nameof(GetGeoAsync), IsSearchable = true, HasLabel = true, LabelCssClass = "overlayed-label", Order = 1)]
        public int? Geo { get; set; }
        public async Task<List<JingetDropDownTreeItemModel>> GetGeoAsync(object? preBindingResult)
            => await new JingetDropDownListTreeElement().BindAsync<GeoModel, int?>(async () =>
            {
                return await Task.FromResult(new List<GeoModel> {
                    new() { Code = 1,ParentId=null, Title = "Iran" },
                    new() { Code = 2,ParentId=null, Title = "USA" },
                    new() { Code = 3,ParentId=1, Title = "Tehran" },
                    new() { Code = 4,ParentId=3, Title = "Tehran City" },
                    new() { Code = 5,ParentId=2, Title = "WA" },
                    new() { Code = 6,ParentId=3, Title = "Pardis" }
                });
            });

        class FlightStatusModel : BaseTypeModel
        {

        }
        class GeoModel : BaseTypeTreeModel<int?>
        {

        }

    }
}
```

***Parameters:***

`Model`: Model used to decorate the form. two-way binding is also supported.

`Rtl`: If set to true, the the form will be rendred Right-to-Left. Default is `true`.

`CustomStyle`: Custom css style used for the whole form.

***Properties:***

`DynamicFields`: Returns a list of fields rendered on page.

`Properties`: return the list of elements used to rednder the dynamic form

***Methods:***

`FindElement`: Find element by id.

`FindElement<TFormElementType>`: Find element by type. 

***Callbacks:***

`OnModelChanged`: This event raised whenever a member's value changed.

`OnModelChangedException`: This event raised whenever an exception occurred while handling OnModelChanged

`OnFieldReady`: This event raised whenever a field rendered on page. This event gives an object of `JingetDynamicField` which contains a property called `RefObject`
which refers to the form element on page. For example in order to get the drop down list items in `OnFieldReady` using `((JingetDropDownList)field.RefObject).Items`.

***Attributes:***

`JingetTextBoxElement`: Render a textbox on the page.

`JingetTextAreaElement`: Render a textarea on the page. Using `Rows` property you can set the textarea rows.

`JingetEmailBoxElement`: Render an email input on the page.

`JingetNumberBoxElement`: Render a number input on the page.

`JingetPasswordBoxElement`: Render a password input on the page.

`JingetTelephoneBoxElement`: Render a tel input on the page.

`JingetColorBoxElement`: Render a color input on the page.

`JingetDateBoxElement`: Render a date input on the page.

`JingetDateTimeLocalBoxElement`: Render a datetime-local input on the page.

`JingetHiddenBoxElement`: Render a hidden input on the page.

`JingetMonthBoxElement`: Render a month input on the page.

`JingetTimeBoxElement`: Render a time input on the page.

`JingetUrlBoxElement`: Render a url input on the page.

`JingetWeekBoxElement`: Render a week input on the page.

`JingetDatePickerElement`: Render a JingetDatePicker on the page. You can set `Culture` and `EnglishNumber` properties.

`JingetDateRangePickerElement`: Render a JingetDateRangePicker on the page. You can set `Culture` and `EnglishNumber` properties.

`JingetDropDownListElement`: Render a select input on the page. If `IsSearchable=true`, then the user can do search among drop down list items. 
Using `PreBindingFunction` user can define a method to run before `BindingFunction`. This method's signature is:

```
public async Task<string> PreBinding(string? token) => await Task.FromResult("This is pre binding");
```

Using `BindingFunction` user can define a method to bind data into drop down list. If `GetTokenBeforeBinding` is set to true, 
then before running the `BindingFunction`, `ITokenStorageService.GetTokenAsync()` method will be called to read the token from localstorage 
where localstorage key is equal to `TokenConfigModel.TokenName`. (See `builder.Services.AddJingetBlazor();`). This method's signature is:

```
public async Task<return type> <Binding function name>(string token, object? preBindingResult)=>...
```

`BindingFunction` can be either `async` or not.

Using `PostBindingFunction` user can define a method to run after `BindingFunction`. This method's signature is:

```
public async Task<string> PostBinding(string? token, object? preBindingResult, object? data) => await Task.FromResult("This is post binding");
```

Full sample for `JingetDropDownListElement` in `DynamicForm` is like below:

```

[JingetDropDownListElement(DisplayName = "Flight Status", IsSearchable = true, DefaultText = "---Choose one---",
BindingFunction = nameof(GetStatusAsync), PreBindingFunction = nameof(PreBinding), PostBindingFunction = nameof(PostBinding),
HasLabel = true, LabelCssClass = "overlayed-label", Order = 1, GetTokenBeforeBinding = true)]
public int? Status { get; init; }
public async Task<string> PreBinding(string? token) 
    => await Task.FromResult("This is pre binding");
public async Task<string> PostBinding(string? token, object? preBindingResult, object? data) 
    => await Task.FromResult("This is post binding");
public async Task<List<JingetDropDownItemModel>> GetStatusAsync(string token, object? preBindingResult)
    => await new JingetDropDownListElement().BindAsync<FlightStatusModel>(async () =>
    {
        return await Task.FromResult(new List<FlightStatusModel>
        {
           new FlightStatusModel{Code= 1,Title= "Boarding" },
           new FlightStatusModel{Code= 2,Title= "Arriving" }
        });
    });

```

`JingetDropDownListTreeElement`: Render a select input containing a tree on the page. Configuration and descriptions for this element is almost same as the `JingetDropDownListElement`.

`JingetLabelElement`: Render a label on the page.

`JingetListElement`: Render a JingetList on the page. List's items are defined using `BodyMember` property. items should be passed as json string to this property.

`JingetTableElement`: Render a JingetTable on the page.(Used in JingetTable component, for more info go to JingetTable component section)

------------

**Jinget DropDownList components**

Add the `JingetDropDownList` to your page and start using it;-)

```
<JingetDropDownList @ref=@ddlSample
                    Id="ddlSample"
                    DataProviderFunc=@GetData
                    DefaultText="---Choose---"
                    HelperText="This is sample drop down list"
                    DisplayName="DDLSample"
                    IsDisabled=false
                    IsReadOnly=false
                    OnChange=@OnChange></JingetDropDownList>
```

***Parameters:***

`Id`: Unique identifier for component in page. This parameter is required.

`Value`: Value entered in the input box

`DisplayName`: If string has value the label text will be displayed in the input, and scaled down at the top if the input has value.

`HelperText`: The HelperText will be displayed below the text field.

`CssClass`: Defines a custom css class used to decorate the component.

`IsDisabled`: Defines wheather the component should be disabled or not.

`IsReadOnly`: If true, the input will be read-only.

`IsRequired`: If true, the input is required.

`IsRtl`: If true, the input will be decorated right to left.

`RequiredError`: Message to be shown whenever the `IsRequired=true` and no value has beed provided.

`IsSearchable`: If true, then user can search among the dropdownlist items.

`DefaultText`: Sets a default text to be shown in dropdownlist before selecting anything

`DataProviderFunc`: Defines a method which is used to populate the data in dropdownlist.

`ParentElementId`: Defines where to attach the dropdown html in page. By default element will be attached to body tag in page.
But in some cases like Bootstrap modals, which tend to steal focus from other elements outside of the modal. 
Since by default, Select2 attaches the dropdown menu to the body element, it is considered "outside of the modal". 
To avoid this problem, you may attach the dropdown to the modal itself by setting the modal id in this parameter.

***Callbacks:***

`OnChange`: Fires a callback whenever the selected item changed.

`OnDataBound`: Fires whenever the `Items` changed.

`OnRendered`: whenever the component rendered on the page.

***Properties:***

`Items`: Contains the items which are populated to dropdownlist using `DataProviderFunc`

`SelectedItem`: Contains the selected item Text and Value.

***Methods:***

`SetSelectedItemAsync`: Select item in dropdownlist based on `Value`

`SetSelectedIndexAsync`: Select item in dropdownlist based on the index in `Items`. Index starts from zero(0).

------------

**Jinget DropDownList Tree components**

Add the `JingetDropDownListTree` to your page and start using it;-)

```
<JingetDropDownListTree @ref=@ddlSampleTree
                    Id="ddlSampleTree"
                    DataProviderFunc=@GetData
                    DefaultText="---Choose---"
                    HelperText="This is sample drop down list tree"
                    DisplayName="DDLSample Tree"
                    IsDisabled=false
                    IsReadOnly=false
                    OnChange=@OnChange></JingetDropDownListTree>
```

***Properties:***

`OriginalItems`: Contains the items which are populated to dropdownlist using `DataProviderFunc`. Data in this property is in the same order as in original data source.

`Items`: Contains a copy of data in `OriginalItems`. Data in this property are NOT in the same order as in original data source.

Other Parameters/properties/callbacks and methods for this components are exactly same as `JingetDropDownList` component. 
Except that `JingetDropDownList` uses `JingetDropDownItemModel` as it's data model provider class which contains `Value` and `Text` properties.
But `JingetDropDownListTree` uses `JingetDropDownTreeItemModel` as data model provider which in addition to `JingetDropDownItemModel` properties, also contains `ParentValue` property to construct the tree structure

------------

**Jinget Input**

Add the `JingetInput` to your page and start using it;-)

```
<JingetInput Id="inpText"
             InputType="InputType.Text"
             @bind-Value="@obj.Name">
</JingetInput>
```

***Parameters:***

`Id`: Unique identifier for component in page. This parameter is required.

`Value`: Value entered in the input box

`DisplayName`: If string has value the label text will be displayed in the input, and scaled down at the top if the input has value.

`CssClass`: Defines a custom css class used to decorate the component.

`IsDisabled`: Defines wheather the component should be disabled or not.

`IsReadOnly`: If true, the input will be read-only.

`IsRequired`: If true, the the input is required.

`RequiredError`: Message to be shown whenever the `IsRequired=true` and no value has beed provided.

`HelperText`: The HelperText will be displayed below the text field.

`Rows`: If rows are greater than 1, then html `testarea` with specified rows number will be rendered on the page.

`InputType`: Defines the input type to be rendered on the page. For example `text`, `password`, `hidden` etc.

***Callbacks:***

`OnChange`: Fires a callback whenever the selected item changed.

------------

**Jinget JsonVisualizer component**

Add the `JingetJsonVisualizer` to your page, call the Visualize method and start using it;-)

*Note: that this component is based on [Alexandre Bodelot](https://github.com/abodelot "Alexandre Bodelot") jquery.json-viewer library.*  [View on GitHub](htthttps://github.com/abodelot/jquery.json-viewerp:// "View on GitHub")

```
<JingetJsonVisualizer @ref=@JsonVisualizer></JingetJsonVisualizer>
```

Fore example:

```
<button @onclick=@Visualize class="btn btn-primary">Visualize</button>
<JingetJsonVisualizer @ref=@jingetJsonVisualizer></JingetJsonVisualizer>
@code {
    JingetJsonVisualizer? jingetJsonVisualizer { get; set; }

    async Task Visualize()
    {
        await jingetJsonVisualizer.Visualize(new { Name = "Vahid", LastName = "Farahmandian", Age = 35, Score = 182.25 });
    }
}
```

***Parameters:***

`Id`: Unique identifier for component in page. This parameter is required.

`Collapsed`: All nodes are collapsed at html generation. default is false.

`RootCollapsable`: Allow root element to be collapsed. default is true.

`WithQuotes`: All JSON keys are surrounded with double quotation marks ({"foobar": 1} instead of {foobar: 1}). default is false.

`WithLinks`: All values that are valid links will be clickable, if false they will only be strings. default is true.

`BigNumbers`: Support different libraries for big numbers, if true, display the real number only, 
false shows object containing big number with all fields instead of number only.

***Methods:***

`Visualize`: Renders the given object in json visualizer

------------

**Jinget Gauge component**

Add the `JingetGauge` to your page and start using it;-)


```
<JingetGauge Id="gauge1"
             HelperText="This is sample gauge"
             DisplayName="Sample Gauge 1"
             GaugeBackGroundColor="#123456"
             GaugeParentElementGroundColor="#ffffff"
             Value=50></JingetGauge>
```

***Parameters:***

`Id`: Unique identifier for component in page. This parameter is required.

`ShowValueAsText`: If set to true the the gauge data-value will be shown as a text below the gauge.

`Width`: Width of gauge. Default is 200px

`HueLow`: Choose the starting hue for the active color (for value 0). Default is 1

`HueHigh`: Choose the ending hue for the active color (for value 100). Default is 128.

`Saturation`: Saturation for active color. Saturation should be represented as a percentage number, such as 100%. Default is 100%.

`Lightness`: Lightness for active color. Lightness should be represented as a percentage number, such as 100%. Default is 50%.

`GaugeBackGroundColor`: Background color of Gauge. Default is #1b1b1f.

`GaugeParentElementGroundColor`: This color should match the parent div of the gauge (or beyond). Default is #323138

***Methods:***

`SetValueAsync`: Set new value to gauge.

------------
## How to Use Services:

**LocalStorage/SessionStorage:**

If you need to work with `localStorage` then you need to inject `ILocalStorageService` and 
if you want to work with `sessionStorage` then you need to inject `ISessionStorageService` to your page or classes.
Both of these methods have same methods with same signatures. Note that these two services are registered in DI container if only you set the `tokenConfigModel` 
to non-null value and also set the `addTokenRelatedServices=true` while adding `builder.Services.AddJingetBlazor();` to `Program.cs`.

****Methods:****

`GetItemAsync`: Get item with specific key from storage.

`GetAllAsync`: Get all items from storage.

`SetItemAsync`: Set item to storage

`UpsertItemAsync`: Add or update item to storage

`RemoveItemAsync`: Remove item with specific key from storage

`RemoveAllAsync`: Remove all items from storage


------------
## How to install
In order to install Jinget Blazor please refer to [nuget.org](https://www.nuget.org/packages/Jinget.Blazor "nuget.org")

## Contact Me
👨‍💻 Twitter: https://twitter.com/_jinget

📧 Email: farahmandian2011@gmail.com

📣 Instagram: https://www.instagram.com/vahidfarahmandian
