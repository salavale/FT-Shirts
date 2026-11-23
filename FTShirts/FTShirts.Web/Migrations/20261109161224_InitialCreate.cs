using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FTShirts.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "shirts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    team = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    is_second_hand = table.Column<bool>(type: "boolean", nullable: false),
                    p_number = table.Column<int>(type: "integer", nullable: true),
                    p_name = table.Column<string>(type: "text", nullable: true),
                    size = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shirts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    username = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.username);
                });

            migrationBuilder.CreateTable(
                name: "shopping_cart",
                columns: table => new
                {
                    username = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shopping_cart", x => x.username);
                    table.ForeignKey(
                        name: "fk_shopping_cart_users_username",
                        column: x => x.username,
                        principalTable: "users",
                        principalColumn: "username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "catalogue_items",
                columns: table => new
                {
                    shirt_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    t_shirt_id = table.Column<int>(type: "integer", nullable: false),
                    offer_type = table.Column<int>(type: "integer", nullable: false),
                    offer_discount = table.Column<int>(type: "integer", nullable: true),
                    tags = table.Column<List<string>>(type: "text[]", nullable: false),
                    shopping_cart_username = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_catalogue_items", x => x.shirt_id);
                    table.ForeignKey(
                        name: "fk_catalogue_items_shirts_t_shirt_id",
                        column: x => x.t_shirt_id,
                        principalTable: "shirts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_catalogue_items_shopping_cart_shopping_cart_username",
                        column: x => x.shopping_cart_username,
                        principalTable: "shopping_cart",
                        principalColumn: "username");
                });

            migrationBuilder.CreateIndex(
                name: "ix_catalogue_items_shopping_cart_username",
                table: "catalogue_items",
                column: "shopping_cart_username");

            migrationBuilder.CreateIndex(
                name: "ix_catalogue_items_t_shirt_id",
                table: "catalogue_items",
                column: "t_shirt_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "catalogue_items");

            migrationBuilder.DropTable(
                name: "shirts");

            migrationBuilder.DropTable(
                name: "shopping_cart");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
