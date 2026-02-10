using Diplomas.Public.DataAccess.Extensions;
using Diplomas.Public.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diplomas.Public.DataAccess
{
    public partial class DiplomasContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            // В случай, че искаме да използваме автоматичното конвертиране
            //modelBuilder.Entity<Diploma>(entity =>
            //{
            //    entity.Property(e => e.Contents).HasJsonConversion<List<KeyValue>>("Contents");
            //});

            //modelBuilder.Entity<BasicDocument>(entity =>
            //{
            //    entity.Property(e => e.Contents).HasJsonConversion<List<PropertyDescription>>("Contents");
            //});
        }
    }
}
