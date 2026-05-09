using Microsoft.Extensions.Localization;
using TaskManagementSystem.Web.Models;

namespace TaskManagementSystem.Web.Helpers;

public static class EnumLocalizer
{
    public static string Translate(Priority value, IStringLocalizer localizer)
        => localizer[$"Priority_{value}"];

    public static string Translate(TaskItemStatus value, IStringLocalizer localizer)
        => localizer[$"Status_{value}"];
}
