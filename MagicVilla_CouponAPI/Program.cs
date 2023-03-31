using AutoMapper;
using FluentValidation;
using MagicVilla_CouponAPI;
using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();


var app = builder.Build();

// Configure the HTTP request pipeline. test
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 404 if non int used/ explicit params
//app.MapGet("/helloworld/{id:int}", (int id) =>
//    {
//        return Results.BadRequest("Id!!!"+id);
//    }
//);
// bad request exception if type not specified in url template

//app.MapGet("/helloworldq/{id}", (int id) =>
//{
//    return Results.BadRequest("Id!!!" + id);
//}
//);

//app.MapPost("/helloworld 2", () => Results.Ok("Hello World 2"));

app.MapGet("/api/coupon", (ILogger<Program> _logger) =>
{
    _logger.Log(LogLevel.Information, "Getting all Coupons");
    return Results.Ok(CouponStore.couponList);
}).WithName("GetCoupons").Produces<IEnumerable<Coupon>>(200);
app.MapGet("/api/coupon/{id:int}", (int id) =>
{
    return Results.Ok(CouponStore.couponList.FirstOrDefault(u => u.Id == id));
}).WithName("GetCoupon").Produces<Coupon>(201).Produces(400);

//app.MapPost("/api/coupon", ([FromBody] CouponCreateDTO coupon_C_DTO) =>
//{
//    // if id available check that it is 0
//    if (string.IsNullOrEmpty(coupon_C_DTO.Name))
//    {
//        return Results.BadRequest("Invalid ID or Coupon Name");
//    }


//    Coupon coupon = new()
//    {
//        IsActive = coupon_C_DTO.IsActive,
//        Name = coupon_C_DTO.Name,
//        Percent = coupon_C_DTO.Percent
//    };

//    if (CouponStore.couponList.FirstOrDefault(u => u.Name.ToLower() == coupon.Name.ToLower()) != null)
//    {
//        return Results.BadRequest("Coupon Name already Exists");
//    }
//    coupon.Id = CouponStore.couponList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
//    CouponStore.couponList.Add(coupon);

//    // convert coupon back to couponDTO to return

//    CouponDTO couponDTO = new()
//    {
//        Id = coupon.Id,
//        IsActive = coupon.IsActive,
//        Name = coupon.Name,
//        Percent = coupon.Percent,
//        Created = coupon.Created
//    };

//    // return location with coupon
//    //return Results.Created($"/api/coupon/{coupon.Id}", coupon);
//    return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, coupon);

//}).WithName("CreateCoupon").Accepts<CouponCreateDTO>("application/json").Produces<CouponDTO>(201).Produces(400);

app.MapPost("/api/coupon", async (ILogger<Program> _logger, IMapper _mapper,
    IValidator<CouponCreateDTO> _validation, [FromBody] CouponCreateDTO coupon_C_DTO) =>
{

    var validationResult = await _validation.ValidateAsync(coupon_C_DTO);
    if (!validationResult.IsValid)
    {
        _logger.Log(LogLevel.Information, "NOT VALID");
        return Results.BadRequest(validationResult.Errors.FirstOrDefault().ToString());
    }
    if (CouponStore.couponList.FirstOrDefault(u => u.Name.ToLower() == coupon_C_DTO.Name.ToLower()) != null)
    {
        return Results.BadRequest("Coupon Name already Exists");
    }

    Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);

    coupon.Id = CouponStore.couponList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
    CouponStore.couponList.Add(coupon);
    CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);
    return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, couponDTO);
    //return Results.Created($"/api/coupon/{coupon.Id}",coupon);
}).WithName("CreateCoupon").Accepts<CouponCreateDTO>("application/json").Produces<CouponDTO>(201).Produces(400);

app.MapPut("/api/coupon", () =>
    {

    });
app.MapDelete("/api/coupon/{id:int}", (int id) =>
{

});


app.UseHttpsRedirection();

app.Run();

