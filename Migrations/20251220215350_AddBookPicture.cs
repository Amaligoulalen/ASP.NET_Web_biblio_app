﻿﻿﻿﻿﻿﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestion_biblio.Migrations
{
    /// <inheritdoc />
    public partial class AddBookPicture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('Books', 'PicturePath') IS NULL
BEGIN
    ALTER TABLE [Books] ADD [PicturePath] nvarchar(max) NULL;
END
");

            migrationBuilder.Sql(@"
IF COL_LENGTH('AspNetUsers', 'FullName') IS NULL
BEGIN
    ALTER TABLE [AspNetUsers] ADD [FullName] nvarchar(max) NULL;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('Books', 'PicturePath') IS NOT NULL
BEGIN
    ALTER TABLE [Books] DROP COLUMN [PicturePath];
END
");

            migrationBuilder.Sql(@"
IF COL_LENGTH('AspNetUsers', 'FullName') IS NOT NULL
BEGIN
    ALTER TABLE [AspNetUsers] DROP COLUMN [FullName];
END
");
        }
    }
}
