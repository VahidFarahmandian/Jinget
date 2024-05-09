using System.ComponentModel.DataAnnotations;

namespace Jinget.Core.Tests._BaseData;

internal class SampleEnum
{
    internal enum ProgrammingLanguage
    {
        [Display(Name = "C#")]
        CSharp = 1,

        [System.ComponentModel.Description("F#.Net")]
        [Display(Name = "F#")]
        FSharp = 2,

        [Display(Name = "C#")]
        VB = 3,

        Golang = 4
    }
}
