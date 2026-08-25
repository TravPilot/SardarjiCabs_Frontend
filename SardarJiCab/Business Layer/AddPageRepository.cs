using Dapper;
using Microsoft.Data.SqlClient;
using SardarJi_Cab_Booking.Models;
using System.Data;

namespace SardarJi_Cab_Booking.Business_Layer
{
   
    public class AddPageRepository : IAddPageRepository
    {
        private readonly IConfiguration _configuration;

        public AddPageRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<AddPageVM?> GetAddPageDetailAsync(AddPageVM addPage)
        {
            var connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            await using var connection = new SqlConnection(connectionString);

            await connection.OpenAsync();

            var parameters = new DynamicParameters();

            parameters.Add(
                "@ClientId",
                addPage.ClientId,
                DbType.Int64,
                ParameterDirection.Input
            );

            parameters.Add(
                "@Url",
                addPage.SeoUrl,
                DbType.String,
                ParameterDirection.Input
            );

            using var multi = await connection.QueryMultipleAsync(
                "GetAddPage_TraviYoPortal",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            // Result Set 1
            var page = await multi.ReadFirstOrDefaultAsync<AddPageVM>();

            if (page == null)
            {
                return null;
            }

            page.ClientId = addPage.ClientId;

            // Result Set 2
            page.Banner = (await multi.ReadAsync<AddPage_Banner>())
                .ToList();

            // Result Set 3
            page.Content = (await multi.ReadAsync<AddPage_Content>())
                .ToList();

            // Get category/additional content
            var categoryParameters = new DynamicParameters();

            categoryParameters.Add(
                "@SeoUrl",
                page.Header,
                DbType.String
            );

            categoryParameters.Add(
                "@ClientId",
                addPage.ClientId,
                DbType.Int64
            );

            //var categoryData = await connection.QueryFirstOrDefaultAsync<AddPageVM>(
            //    "GetAddPageDetailswithCategory",
            //    categoryParameters,
            //    commandType: CommandType.StoredProcedure
            //);

            //if (categoryData != null &&
            //    !string.IsNullOrEmpty(categoryData.AddPage))
            //{
            //    page.AddpageContent =
            //        Newtonsoft.Json.JsonConvert
            //            .DeserializeObject<List<AddPageVM>>(
            //                categoryData.AddPage.Replace("URL", "SeoUrl")
            //            ) ?? new List<AddPageVM>();
            //}

            return page;
        }
    }
}
