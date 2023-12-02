# Jinget Blazor
Blazor components optimised for RTL languaged such as Farsi, Arabic etc.

**Components list:**
- [x] **Captcha**
- [ ] Jalali Date Picker (*On the way to open source*)
- [ ] Jalali Date Range Picker (*On the way to open source*)
- [ ] Message Box (*On the way to open source*)
- [ ] Modal (*On the way to open source*)
- [ ] List (*On the way to open source*)
- [ ] Table (*On the way to open source*)
- [ ] Dynamic Form (*On the way to open source*)

## How to Use:

Download the package from NuGet using Package Manager:
`Install-Package Jinget.Blazor`
You can also use other methods supported by NuGet. Check [Here](https://www.nuget.org/packages/Jinget.Blazor "Here") for more information.

## Configuration

**Jinget Captcha component**

***References:***

Add reference to `jinget.captcha.css` in your `_Host.razor` or `App.razor` files.

```
<link href="_content/Jinget.Blazor/css/jinget.captcha.css" rel="stylesheet" />
```

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

`IsValid`: Checks if the given input text is squal to the image's text

`GetNewCaptchaAsync`: Draw a new captcha image

------------
## How to install
In order to install Jinget Blazor please refer to [nuget.org](https://www.nuget.org/packages/Jinget.Blazor "nuget.org")

## Contact Me
👨‍💻 Twitter: https://twitter.com/_jinget

📧 Email: farahmandian2011@gmail.com

📣 Instagram: https://www.instagram.com/vahidfarahmandian
