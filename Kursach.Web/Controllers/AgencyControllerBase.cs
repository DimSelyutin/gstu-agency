using System.Text.Json;
using Kursach.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Kursach.Web.Controllers;

/// <summary>
/// Базовый контроллер с ReturnUrl и сохранением фильтров в Session.
/// </summary>
public abstract class AgencyControllerBase : Controller
{
    /// <summary>Ключ TempData для URL возврата к списку с фильтрами.</summary>
    protected const string ReturnUrlKey = "ReturnUrl";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Сохраняет URL возврата из referrer или query.
    /// </summary>
    protected void RememberReturnUrl(string? returnUrl = null)
    {
        if (TempData is null)
        {
            return;
        }

        var url = returnUrl
            ?? Request.Query["returnUrl"].FirstOrDefault()
            ?? Request.Headers.Referer.FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(url) && Url?.IsLocalUrl(url) == true)
        {
            TempData[ReturnUrlKey] = url;
        }
    }

    /// <summary>
    /// Возвращает redirect на сохранённый URL или на указанное действие.
    /// </summary>
    protected IActionResult RedirectToReturnOr(string actionName, object? routeValues = null)
    {
        if (TempData is not null
            && TempData.TryGetValue(ReturnUrlKey, out var value)
            && value is string url
            && Url?.IsLocalUrl(url) == true)
        {
            return Redirect(url);
        }

        return RedirectToAction(actionName, routeValues);
    }

    /// <summary>
    /// Восстанавливает фильтр из Session, если в запросе нет параметров фильтрации;
    /// иначе сохраняет текущий фильтр. При reset=true очищает Session.
    /// </summary>
    protected T ResolveFilter<T>(T filter, string? reset = null) where T : FilterParams, new()
    {
        var key = $"Filter:{ControllerContext.ActionDescriptor.ControllerName}";

        if (string.Equals(reset, "1", StringComparison.OrdinalIgnoreCase)
            || string.Equals(reset, "true", StringComparison.OrdinalIgnoreCase))
        {
            HttpContext.Session.Remove(key);
            return new T();
        }

        var hasQueryFilter = Request.Query.Keys.Any(k =>
            !string.Equals(k, "reset", StringComparison.OrdinalIgnoreCase));

        if (!hasQueryFilter)
        {
            var json = HttpContext.Session.GetString(key);
            if (!string.IsNullOrWhiteSpace(json))
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(json, JsonOptions) ?? filter;
                }
                catch (JsonException)
                {
                    HttpContext.Session.Remove(key);
                }
            }

            return filter;
        }

        HttpContext.Session.SetString(key, JsonSerializer.Serialize(filter, JsonOptions));
        return filter;
    }

    /// <summary>
    /// Добавляет сообщение об ошибке удаления из-за связанных записей.
    /// </summary>
    protected void AddDeleteConstraintError()
    {
        ModelState.AddModelError(string.Empty,
            "Нельзя удалить запись: есть связанные данные. Сначала удалите или измените связанные объекты.");
    }
}
