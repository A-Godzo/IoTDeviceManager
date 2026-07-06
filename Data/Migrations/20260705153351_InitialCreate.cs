using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IoTDeviceManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeploymentSites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SiteName = table.Column<string>(type: "TEXT", nullable: false),
                    PhysicalAddress = table.Column<string>(type: "TEXT", nullable: true),
                    NetworkSubnet = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeploymentSites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SensorModules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SensorType = table.Column<string>(type: "TEXT", nullable: false),
                    CommunicationProtocol = table.Column<string>(type: "TEXT", nullable: false),
                    Manufacturer = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorModules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Microcontrollers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MACAddress = table.Column<string>(type: "TEXT", nullable: false),
                    ChipModel = table.Column<string>(type: "TEXT", nullable: false),
                    FirmwareVersion = table.Column<string>(type: "TEXT", nullable: true),
                    LastSeenAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeploymentSiteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Microcontrollers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Microcontrollers_DeploymentSites_DeploymentSiteId",
                        column: x => x.DeploymentSiteId,
                        principalTable: "DeploymentSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataPin = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    MicrocontrollerId = table.Column<int>(type: "INTEGER", nullable: false),
                    SensorModuleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceConfigurations_Microcontrollers_MicrocontrollerId",
                        column: x => x.MicrocontrollerId,
                        principalTable: "Microcontrollers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceConfigurations_SensorModules_SensorModuleId",
                        column: x => x.SensorModuleId,
                        principalTable: "SensorModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TelemetryLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MicrocontrollerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Payload = table.Column<string>(type: "TEXT", nullable: false),
                    Severity = table.Column<int>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelemetryLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelemetryLogs_Microcontrollers_MicrocontrollerId",
                        column: x => x.MicrocontrollerId,
                        principalTable: "Microcontrollers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceConfigurations_MicrocontrollerId",
                table: "DeviceConfigurations",
                column: "MicrocontrollerId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceConfigurations_SensorModuleId",
                table: "DeviceConfigurations",
                column: "SensorModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Microcontrollers_DeploymentSiteId",
                table: "Microcontrollers",
                column: "DeploymentSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryLogs_MicrocontrollerId",
                table: "TelemetryLogs",
                column: "MicrocontrollerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceConfigurations");

            migrationBuilder.DropTable(
                name: "TelemetryLogs");

            migrationBuilder.DropTable(
                name: "SensorModules");

            migrationBuilder.DropTable(
                name: "Microcontrollers");

            migrationBuilder.DropTable(
                name: "DeploymentSites");
        }
    }
}
