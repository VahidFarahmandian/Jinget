using SkiaSharp;

namespace Jinget.Blazor.Components.Captcha;

public class JingetCaptcha : ComponentBase
{
    class Letter
    {
        public int Angle { get; set; }
        public string? Value { get; set; }
        public SKColor ForeColor { get; set; }
        public string? FontFamily { get; set; }
    }

    public enum CharSetCaseSensitivityOptions : byte
    {
        /// <summary>
        /// Only upper case characters will be used for rendering
        /// </summary>
        UpperCaseOnly,

        /// <summary>
        /// Only lower case characters will be used for rendering
        /// </summary>
        LowerCaseOnly,

        /// <summary>
        /// Both upper and lower case characters will be used for rendering
        /// </summary>
        IgnoreCase
    }

    public enum CharSetTypeOptions : byte
    {
        /// <summary>
        /// Only digits will be used for rendering(0-9)
        /// </summary>
        DigitOnly,

        /// <summary>
        /// Only English characters will be used for rendering
        /// </summary>
        EnglishCharsOnly,

        /// <summary>
        /// Only Farsi characters will be used for rendering.
        /// It is important to set the <seealso cref="IsRtlCharSet"/>=true for currect rendering. 
        /// Otherwise characters will be rendered in reverse order
        /// </summary>
        FarsiCharsOnly,

        /// <summary>
        /// Only English characters and digits will be used for rendering.
        /// </summary>
        EnglishCharsPlusDigit,

        /// <summary>
        /// Only Farsi characters and digits will be used for rendering.
        /// It is important to set the <seealso cref="IsRtlCharSet"/>=true for currect rendering. 
        /// Otherwise characters will be rendered in reverse order
        /// </summary>
        FarsiCharsPlusDigit,

        /// <summary>
        /// Only Farsi and English characters will be used for rendering.
        /// It is important to set the <seealso cref="IsRtlCharSet"/>=true for currect rendering. 
        /// Otherwise characters will be rendered in reverse order.
        /// Because of UX issue, this combination is not recomended
        /// </summary>
        EnglishCharsPlusFarsiChars,

        /// <summary>
        /// All the Farsi and English characters and digits will be used for rendering.
        /// It is important to set the <seealso cref="IsRtlCharSet"/>=true for currect rendering. 
        /// Otherwise characters will be rendered in reverse order.
        /// Because of UX issue, this combination is not recomended
        /// </summary>
        BlendAll,

        /// <summary>
        /// characters provided by <seealso cref="CustomCharSet"/> will be used for rendering.
        /// </summary>
        Custom
    }

    List<char> GetEnglishChars() => ContentCaseSensitivity switch
    {
        CharSetCaseSensitivityOptions.LowerCaseOnly => [.. StringUtility.GetEnglishChars(true).ToCharArray()],
        CharSetCaseSensitivityOptions.UpperCaseOnly => [.. StringUtility.GetEnglishChars(false).ToCharArray()],
        CharSetCaseSensitivityOptions.IgnoreCase => [.. StringUtility.GetEnglishChars(true).ToCharArray(), .. StringUtility.GetEnglishChars(false).ToCharArray()],
        _ => [],
    };

    string GetCaptchaWord(int length)
    {
        List<char> chars = CharSetType switch
        {
            CharSetTypeOptions.DigitOnly => [.. StringUtility.GetEnglishDigits().ToCharArray()],
            CharSetTypeOptions.EnglishCharsOnly => GetEnglishChars(),
            CharSetTypeOptions.FarsiCharsOnly => StringUtility.GetFarsiChars(),
            CharSetTypeOptions.EnglishCharsPlusDigit => [.. GetEnglishChars(), .. StringUtility.GetEnglishDigits().ToCharArray()],
            CharSetTypeOptions.FarsiCharsPlusDigit => [.. StringUtility.GetFarsiChars(), .. StringUtility.GetEnglishDigits().ToCharArray()],
            CharSetTypeOptions.EnglishCharsPlusFarsiChars => [.. GetEnglishChars(), .. StringUtility.GetFarsiChars()],
            CharSetTypeOptions.BlendAll => [.. GetEnglishChars(), .. StringUtility.GetFarsiChars(), .. StringUtility.GetEnglishDigits().ToCharArray()],
            CharSetTypeOptions.Custom => [.. CustomCharSet.ToCharArray()],
            _ => []
        };

        var random = new Random(DateTime.Now.Millisecond);
        return new(Enumerable.Repeat(chars, length)
                                  .Select(s => s[random.Next(s.Count)])
                                  .ToArray());
    }

    /// <summary>
    /// Captcha image box width.
    /// Default is 170
    /// </summary>
    [Parameter] public int Width { get; set; } = 170;

    /// <summary>
    /// Captcha image box height.
    /// Default is 40
    /// </summary>
    [Parameter] public int Height { get; set; } = 40;

    int charCount = 5;
    /// <summary>
    /// Total number of characters to be rendered in image.
    /// Default value is 5. The value should be greater than 0 otherwise it will be replaced by 5
    /// </summary>
    [Parameter]
    public int CharCount
    {
        get => charCount; set
        {
            if (value <= 0)
                value = 5;
            charCount = value;
        }
    }

    /// <summary>
    /// Set the character set used for rendering the image.
    /// Default is <seealso cref="CharSetTypeOptions.DigitOnly"/>
    /// </summary>
    [Parameter] public CharSetTypeOptions CharSetType { get; set; } = CharSetTypeOptions.DigitOnly;

    /// <summary>
    /// If <seealso cref="ContentCharsType"/> is set to <seealso cref="CharsType.Custom"/>
    /// then characters provided by <seealso cref="CustomCharSet"/> will be used to render the image
    /// </summary>
    [Parameter] public string CustomCharSet { get; set; } = "";

    /// <summary>
    /// Set the character set case sensitivity used for rendering the image.
    /// This property is only applied to the English characters.
    /// Default value is <seealso cref="CharSetCaseSensitivityOptions.IgnoreCase"/>
    /// </summary>
    [Parameter] public CharSetCaseSensitivityOptions ContentCaseSensitivity { get; set; } = CharSetCaseSensitivityOptions.IgnoreCase;

    /// <summary>
    /// Check if the input captcha text validation should be done in case sensitive or case insensitive manner.
    /// </summary>
    [Parameter] public bool CaseSesitiveComparison { get; set; }

    /// <summary>
    /// If RTL language's character set is being used(such as Farsi, Arabic, etc), 
    /// this property should be set to true.
    /// </summary>
    [Parameter] public bool IsRtlCharSet { get; set; }

    /// <summary>
    /// Sets the Font families used for drawing the text inside the image.
    /// Default Values are: Arial, Tahoma and Times New Roman
    /// </summary>
    [Parameter] public string[] FontFamilies { get; set; } = ["Arial", "Tahoma", "Times New Roman"];

    /// <summary>
    /// Fires a callback whenever the captcha image is changed
    /// </summary>
    [Parameter] public EventCallback<string> CaptchaChanged { get; set; }

    string? captchaWord;
    Random? randomValue;
    List<Letter> letters = [];
    SKColor bgColor;

    protected override async Task OnInitializedAsync()
    {
        await GetNewCaptchaAsync();
        await base.OnInitializedAsync();
    }

    void Initialization()
    {
        if (string.IsNullOrEmpty(captchaWord)) return;

        randomValue = new Random();
        bgColor = new SKColor((byte)randomValue.Next(70, 100),
                                (byte)randomValue.Next(60, 80),
                                (byte)randomValue.Next(50, 90)
                              );

        letters = [];

        if (!string.IsNullOrEmpty(captchaWord))
        {
            var chars = IsRtlCharSet ? captchaWord.Reverse() : captchaWord;

            foreach (char c in chars)
            {
                var letter = new Letter
                {
                    Value = c.ToString(),
                    Angle = randomValue.Next(-15, 25),
                    ForeColor = new SKColor((byte)randomValue.Next(100, 256),
                                            (byte)randomValue.Next(110, 256),
                                            (byte)randomValue.Next(90, 256)),
                    FontFamily = FontFamilies[randomValue.Next(0, FontFamilies.Length)],
                };

                letters.Add(letter);
            }
        }
    }

    string DrawImage()
    {
        SKImageInfo imageInfo = new(Width, Height);
        using var surface = SKSurface.Create(imageInfo);
        var canvas = surface.Canvas;
        canvas.Clear(bgColor);

        using (SKPaint paint = new())
        {
            //starting point(x axis)
            float x = 25;

            foreach (Letter l in letters)
            {
                paint.Color = l.ForeColor;
                paint.Typeface = SKTypeface.FromFamilyName(l.FontFamily);

                paint.TextAlign = IsRtlCharSet ? SKTextAlign.Right : SKTextAlign.Left;
                paint.TextSize = randomValue.Next(Height / 2, Height / 2 + Height / 4);
                paint.FakeBoldText = true;
                paint.IsAntialias = true;

                SKRect rect = new();
                float width = paint.MeasureText(l.Value, ref rect);

                float textWidth = width;
                var y = Height - (rect.Height - 5);

                canvas.Save();

                canvas.RotateDegrees(l.Angle, x, y);
                canvas.DrawText(l.Value, x, y, paint);

                canvas.Restore();

                x += textWidth + 10;
            }

            canvas.DrawLine(0, randomValue.Next(0, Height), Width, randomValue.Next(0, Height), paint);
            canvas.DrawLine(0, randomValue.Next(0, Height), Width, randomValue.Next(0, Height), paint);
            paint.Style = SKPaintStyle.Stroke;
            canvas.DrawOval(randomValue.Next(-Width, Width), randomValue.Next(-Height, Height), Width, Height, paint);
        }

        // save the file
        MemoryStream memoryStream = new();
        using (var image = surface.Snapshot())
        using (var data = image.Encode(SKEncodedImageFormat.Png, 75))
            data.SaveTo(memoryStream);
        string imageBase64Data2 = Convert.ToBase64String(memoryStream.ToArray());
        return string.Format("data:image/gif;base64,{0}", imageBase64Data2);
    }

    void AddImageToRenderTree(RenderTreeBuilder builder, string img)
    {
        var seq = 0;
        builder.OpenElement(++seq, "div");
        builder.AddAttribute(++seq, "class", "divCaptcha");
        {
            builder.OpenElement(++seq, "img");
            builder.AddAttribute(++seq, "src", img);
            builder.CloseElement();

            builder.OpenElement(++seq, "button");
            {
                builder.AddAttribute(++seq, "class", "btn-refresh");
                builder.AddAttribute(++seq, "style", $"height :{Height}px");
                builder.AddAttribute(++seq, "type", "button");
                builder.AddAttribute(++seq, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, async () => await GetNewCaptchaAsync()));
            }
            builder.CloseElement();
        }
        builder.CloseElement();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (randomValue == null) return;
        if (string.IsNullOrEmpty(captchaWord)) return;

        AddImageToRenderTree(builder, DrawImage());

        base.BuildRenderTree(builder);
    }

    /// <summary>
    /// Draw a new captcha image
    /// </summary>
    public async Task GetNewCaptchaAsync()
    {
        captchaWord = GetCaptchaWord(CharCount);
        Initialization();
        await CaptchaChanged.InvokeAsync(captchaWord);
        StateHasChanged();
    }

    /// <summary>
    /// Checks if the given input text is squal to the image's text
    /// </summary>
    public bool IsValid(string input) => CaseSesitiveComparison
            ? string.Equals(captchaWord, input, StringComparison.Ordinal)
            : string.Equals(captchaWord, input, StringComparison.OrdinalIgnoreCase);
}