using FluentAssertions;
using Kursach.Web.Controllers;
using Kursach.Web.Models.Entities;
using Kursach.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace Kursach.Tests.Controllers;

/// <summary>
/// Тесты контроллера сфер деятельности.
/// </summary>
public class IndustriesControllerTests
{
    private static IndustriesController CreateController(Mock<IIndustryService> service)
    {
        var controller = new IndustriesController(service.Object);
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new TestSession();
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        return controller;
    }

    [Fact]
    public async Task Create_Post_InvalidModel_ReturnsView()
    {
        var service = new Mock<IIndustryService>();
        var controller = CreateController(service);
        controller.ModelState.AddModelError("Name", "required");

        var result = await controller.Create(new Industry(), CancellationToken.None);

        result.Should().BeOfType<ViewResult>();
        service.Verify(s => s.CreateAsync(It.IsAny<Industry>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Create_Post_ValidModel_Redirects()
    {
        var service = new Mock<IIndustryService>();
        service.Setup(s => s.CreateAsync(It.IsAny<Industry>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var controller = CreateController(service);
        var result = await controller.Create(new Industry { Name = "IT" }, CancellationToken.None);

        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be(nameof(IndustriesController.Index));
    }

    [Fact]
    public async Task Details_UnknownId_ReturnsNotFound()
    {
        var service = new Mock<IIndustryService>();
        service.Setup(s => s.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Industry?)null);

        var controller = CreateController(service);
        var result = await controller.Details(99, CancellationToken.None);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteConfirmed_CallsService_AndRedirects()
    {
        var service = new Mock<IIndustryService>();
        service.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var controller = CreateController(service);
        var result = await controller.DeleteConfirmed(1, CancellationToken.None);

        result.Should().BeOfType<RedirectToActionResult>();
        service.Verify(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    private sealed class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _store = new();

        public string Id => "test";
        public bool IsAvailable => true;
        public IEnumerable<string> Keys => _store.Keys;
        public void Clear() => _store.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _store.Remove(key);
        public void Set(string key, byte[] value) => _store[key] = value;
        public bool TryGetValue(string key, out byte[]? value) => _store.TryGetValue(key, out value);
    }
}
