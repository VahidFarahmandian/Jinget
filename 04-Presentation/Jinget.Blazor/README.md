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

**Services list:**
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
<JingetTable 
    Model=@ModelRtl
    SearchProviderAsync=@SearchAsyncRtl
    ShowPagination=true
    ShowSearchBar=true>
        <ActionContent>
            <MudButton 
                OnClick=@(async ()=> await ShowDetailFarsi((SampleDataFarsi)context))
                ButtonType="ButtonType.Reset"
                Color="Color.Warning">Details...
            </MudButton>
            </ActionContent>
</JingetTable>
...
@code {
    JingetMessageBox messageBox;
    TableData<SampleDataFarsi> ModelRtl;
    protected override void OnInitialized() { 
        ModelRtl = GetDataFarsi();
    }

    async Task ShowDetailFarsi(SampleDataFarsi data){
        messageBox.CloseButtonText =  "بستن";
        await messageBox.ShowInfoAsync(
        data.Id.ToString(), 
        $"{data.Name} {data.LastName}",
        System.Text.Json.JsonSerializer.Serialize(data), rtl:true);
    }

    async Task<TableData<SampleDataFarsi>> SearchAsyncRtl(TableState state, string? searchString = null)
    {
        try
        {
            IQueryable<SampleDataFarsi> data;
            if (searchString == null)
                data=GetDataFarsi().Items.AsQueryable();
            else
                data = GetDataFarsi().Items
                .Where(x =>
                    x.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase) ||
                    x.LastName.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)).AsQueryable();

            int totalCount =data==null?0: data.Count();

            if(!string.IsNullOrWhiteSpace(state.SortLabel)){
                var sortDirection = state.SortDirection == SortDirection.Ascending ?
                OrderByDirection.Ascending:
                OrderByDirection.Descending;
                
                data = data.OrderByDynamic(state.SortLabel,sortDirection);
            }

            var response= data
            .Skip(state.Page*state.PageSize)
            .Take(state.PageSize).ToList();

            if (response == null || response.Count==0)
                return JingetObjectFactory<SampleDataFarsi>.EmptyTableData.Instance;

            return await Task.FromResult( 
                new TableData<SampleDataFarsi>
                {
                    Items = response,
                    TotalItems = totalCount
                });
        }
        catch (Exception ex)
        {
            await messageBox.ShowErrorAsync("Error", "Unable to search data", ex.Message);
        }
        return JingetObjectFactory<SampleDataFarsi>.EmptyTableData.Instance;
    }

    TableData<SampleDataFarsi> GetDataFarsi()
    {
        var data = new List<SampleDataFarsi>
            {
                new SampleDataFarsi(1,"وحید","فرهمندیان",34,true),
                new SampleDataFarsi(2,"علی","احمدی",40,true),
                new SampleDataFarsi(3,"محسن","نوروزی",18,true),
                new SampleDataFarsi(4,"قانعی","مریم",24,false),
                new SampleDataFarsi(5,"سارا","حسین زاده",37,false),
                new SampleDataFarsi(6,"امیر","رحمانی",29,true),
                new SampleDataFarsi(7,"سید رحمان","رئوفی اصل",54,true),
                new SampleDataFarsi(8,"سامان","صادقی",41,true),
                new SampleDataFarsi(9,"ابوالفضل","بهنام پور",19,true),
                new SampleDataFarsi(10,"ژاله","علیزاده",38,false),
                new SampleDataFarsi(11,"سیدرضا","ابوالفتحی",47,false),
                new SampleDataFarsi(12,"مهتاب","آسمانی",26,true),
                new SampleDataFarsi(13,"ستاره","فضائی",71,true),
                new SampleDataFarsi(14,"امیررضا","عسکری",30,false),
                new SampleDataFarsi(15,"عرفان","طباطبائی",25,true),
                new SampleDataFarsi(16,"پانته آ","قوام",31,false),
                new SampleDataFarsi(17,"یحیی","فرهمند",18,false),
                new SampleDataFarsi(18,"ناصر","ملک زاده",24,true),
            };
        return new TableData<SampleDataFarsi>
            {
                Items = data,
                TotalItems = data.Count
            };
    }

    [JingetTableElement]
    class SampleDataFarsi
    {
        public SampleDataFarsi(int id, string name, string lastname,int age, bool isActive)
        {
            Id=id;
            Name=name;
            LastName=lastname;
            Age = age;
            IsActive = isActive;
        }

        [JingetTableMember(DisplayName = "#")]
        public int Id { get; set; }
        
        [JingetTableMember(DisplayName = "Name")]
        public string Name{get;set;}

        [JingetTableMember(DisplayName = "Last name")]
        public string LastName{get;set;}

        [JingetTableMember(DisplayName = "Age", Sortable=true)]
        public int Age{get;set;}

        [JingetTableMember(DisplayName = "Status")]
        public bool IsActive{get;set;}
    }

}
```

***Parameters:***

`Model`: Model used to bind to the component. Class defining the model should have `JingetTable` attributes. Also each property used to rendered as table's column should have `JingetTableMember` attribute. 

`ShowSearchBar`: Defines whether to show the search bar or not

`Rtl`: If set to true, the the modal content will be rendred Right-to-Left. Default is `true`

`ShowPagination`: Defines whether to show the pagination bar or not

`ActionContent`: Defines the content which should be rendered in each rows as custom actions.

`SearchBarContent`: Defines the custom content which should be rendered in search bar.

`SearchProviderAsync`: Defines a Func delegate to do the search action.

`PaginationSetting`: Settings used for table's pagination section.

`NoRecordText`: Text used to show when there is no data to display in table.

`SearchPlaceHolderText`: Text used to shown in search bar input box.

***Methods:***

`Reload`: Used to reload the data into the table.

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

        [JingetLabelElement(DisplayName = "Score", HasLabel = false)]
        public int Score { get; init; } = 1850;

        [JingetTextAreaElement(DisplayName = "More info", Rows =3)]
        public string Description { get; init; }

        [JingetNumberBoxElement(DisplayName = "Age", Order =7)]
        public int Age { get; set; }

        [JingetDropDownListElement(DisplayName = "Flight Status", BindingFunction = nameof(GetStatusAsync), 
        IsSearchable =true,DefaultText ="---Choose one---",HasLabel =true, LabelCssClass = "overlayed-label", Order =8)]
        public int? Status { get; init; }
        public async Task<List<JingetDropDownItemModel>> GetStatusAsync()
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

Add the `JingetJsonVisualizer` to your page and start using it;-)

*Note: that this component is based on [Alexandre Bodelot](https://github.com/abodelot "Alexandre Bodelot") jquery.json-viewer library.*  [View on GitHub](htthttps://github.com/abodelot/jquery.json-viewerp:// "View on GitHub")

```
<JingetJsonVisualizer @ref=@JsonVisualizer></JingetJsonVisualizer>
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
