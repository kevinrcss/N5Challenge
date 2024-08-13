using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using N5Challenge.Application.DTOs.Permission;
using N5Challenge.Application.Services;
using N5Challenge.Core.Entities;
using N5Challenge.Core.Interfaces;
using N5Challenge.Infrastructure.Kafka;
using N5Challenge.Infrastructure.Services;
using N5Challenge.Infrastructure.Settings;

namespace N5Challenge.UnitTests.Services
{
    public class PermissionServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IElasticsearchService> _mockElasticsearchService;
        private readonly Mock<IKafkaProducer> _mockKafkaProducer;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IOptions<KafkaSettings>> _mockKafkaSettings;
        private readonly Mock<ILogger<PermissionService>> _mockLogger;
        private readonly PermissionService _permissionService;

        public PermissionServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockElasticsearchService = new Mock<IElasticsearchService>();
            _mockKafkaProducer = new Mock<IKafkaProducer>();
            _mockMapper = new Mock<IMapper>();
            _mockKafkaSettings = new Mock<IOptions<KafkaSettings>>();
            _mockLogger = new Mock<ILogger<PermissionService>>();

            _mockKafkaSettings.Setup(x => x.Value).Returns(new KafkaSettings { Topic = "test-topic" });

            _permissionService = new PermissionService(
                _mockUnitOfWork.Object,
                _mockMapper.Object,
                _mockLogger.Object,
                _mockElasticsearchService.Object,
                _mockKafkaProducer.Object,
                _mockKafkaSettings.Object
            );
        }

        [Fact]
        public async Task CreatePermissionAsync()
        {
            // Arrange
            var permissionDto = new PermissionCreateDto
            {
                EmployeeName = "John",
                EmployeeLastName = "Doe",
                PermissionTypeId = 1,
                PermissionDate = DateTime.Now
            };

            var permission = new Permission
            {
                Id = 1,
                EmployeeName = "John",
                EmployeeLastName = "Doe",
                PermissionTypeId = 1,
                PermissionDate = DateTime.Now
            };

            _mockMapper.Setup(m => m.Map<Permission>(It.IsAny<PermissionCreateDto>())).Returns(permission);
            _mockMapper.Setup(m => m.Map<PermissionCreateDto>(It.IsAny<Permission>())).Returns(permissionDto);

            _mockUnitOfWork.Setup(uow => uow.Permissions.AddAsync(It.IsAny<Permission>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(1);

            _mockElasticsearchService.Setup(es => es.IndexPermissionAsync(It.IsAny<Permission>())).ReturnsAsync(true);

            _mockKafkaProducer.Setup(kp => kp.ProduceAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>())).Returns(Task.CompletedTask);

            // Act
            var result = await _permissionService.RequestPermissionAsync(permissionDto);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task ModifyPermissionAsync()
        {
            // Arrange
            var permissionDto = new PermissionDto
            {
                Id = 1,
                EmployeeName = "John",
                EmployeeLastName = "Doe",
                PermissionTypeId = 1,
                PermissionDate = DateTime.Now
            };

            var permission = new Permission
            {
                Id = 1,
                EmployeeName = "John",
                EmployeeLastName = "Doe",
                PermissionTypeId = 1,
                PermissionDate = DateTime.Now
            };

            _mockUnitOfWork.Setup(uow => uow.Permissions.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(permission);
            _mockMapper.Setup(m => m.Map(It.IsAny<PermissionDto>(), It.IsAny<Permission>())).Returns(permission);
            _mockMapper.Setup(m => m.Map<PermissionDto>(It.IsAny<Permission>())).Returns(permissionDto);

            _mockUnitOfWork.Setup(uow => uow.Permissions.Update(It.IsAny<Permission>()));
            _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync()).ReturnsAsync(1);

            _mockElasticsearchService.Setup(es => es.UpdatePermissionAsync(It.IsAny<Permission>())).ReturnsAsync(true);

            _mockKafkaProducer.Setup(kp => kp.ProduceAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>())).Returns(Task.CompletedTask);

            // Act
            var result = await _permissionService.ModifyPermissionAsync(permissionDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(permissionDto, result.Data);
        }

        [Fact]
        public async Task GetPermissionsAsync()
        {
            // Arrange
            var permissions = new List<Permission>
            {
                new Permission { Id = 1, EmployeeName = "John", EmployeeLastName = "Doe", PermissionTypeId = 1, PermissionDate = DateTime.Now },
                new Permission { Id = 2, EmployeeName = "Jane", EmployeeLastName = "Doe", PermissionTypeId = 2, PermissionDate = DateTime.Now }
            };

            var permissionDtos = new List<PermissionDto>
            {
                new PermissionDto { Id = 1, EmployeeName = "John", EmployeeLastName = "Doe", PermissionTypeId = 1, PermissionDate = DateTime.Now },
                new PermissionDto { Id = 2, EmployeeName = "Jane", EmployeeLastName = "Doe", PermissionTypeId = 2, PermissionDate = DateTime.Now }
            };

            var mockPermissionRepository = new Mock<IRepository<Permission>>();
            mockPermissionRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(permissions);

            _mockUnitOfWork.Setup(uow => uow.Permissions).Returns(mockPermissionRepository.Object);

            _mockElasticsearchService.Setup(es => es.GetAllPermissionsAsync()).ReturnsAsync(permissions);
            _mockMapper.Setup(m => m.Map<IEnumerable<PermissionDto>>(It.IsAny<IEnumerable<Permission>>())).Returns(permissionDtos);

            _mockKafkaProducer.Setup(kp => kp.ProduceAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>())).Returns(Task.CompletedTask);

            // Act
            var result = await _permissionService.GetPermissionsAsync();

            // Assert
            Assert.True(result.Success);
            Assert.Equal(permissionDtos, result.Data);
        }
    }
}
