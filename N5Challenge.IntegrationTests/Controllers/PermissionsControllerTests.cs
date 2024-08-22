using N5Challenge.Application.Common;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Net;
using N5Challenge.Application.DTOs.Permission;

namespace N5Challenge.IntegrationTests.Controllers
{
    public class PermissionsControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public PermissionsControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetPermissions()
        {
            //// Act
            var response = await _client.GetAsync("/api/permissions");

            // Assert
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Result<IEnumerable<PermissionDto>>>(content);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
        }

        [Fact]
        public async Task Create()
        {
            // Arrange
            var newPermission = new PermissionCreateDto
            {
                EmployeeName = "Test",
                EmployeeLastName = "User",
                PermissionTypeId = 1,
                PermissionDate = DateTime.Now
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/permissions", newPermission);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<Result<PermissionDto>>();
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(newPermission.EmployeeName, result.Data.EmployeeName);
        }

        [Fact]
        public async Task Modify()
        {
            // Arrange
            var modifiedPermission = new PermissionUpdateDto
            {
                Id = 1,
                EmployeeName = "Modified",
                EmployeeLastName = "User",
                PermissionTypeId = 2,
                PermissionDate = DateTime.Now
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/permissions/{modifiedPermission.Id}", modifiedPermission);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<Result<PermissionDto>>();
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(modifiedPermission.EmployeeName, result.Data.EmployeeName);
        }


    }
}
