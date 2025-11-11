using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LautusInformatica.Migrations
{
    /// <inheritdoc />
    public partial class SuperAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"call sp_CreateUser('Admin', '123456', '9999999999', 'admin@lautus.com',
                                    0, 'Rua 123', 0, @a);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
