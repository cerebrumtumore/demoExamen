var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
var app = builder.Build();
app.UseCors(o => o
.AllowAnyOrigin()
.AllowAnyMethod()
.AllowAnyHeader());

bool isUpdatedStatus = false;
string message = "";
List<Order> repo = new List<Order>()
{
    new Order(1, "2000-5-1" ,"Принтер", "hp-123132", "Не печатает", "Антон Львович", "791111111", "В процессе", "КОЛЯ")
};

app.MapGet("/", () =>
{
    if (isUpdatedStatus)
    {
        string buffer = message;
        isUpdatedStatus = false;
        message = "";
        return Results.Json(new OrderGetWithMessage(repo, buffer));
    }
    else
        return Results.Json(repo);

});

app.MapPost("/create", (Order order) => repo.Add(order));

app.MapGet("/filter/{param}", (string param) => repo.FindAll(o =>
o.Model == param ||
o.DescriptionProblem == param ||
o.Client == param ||
o.Status == param ||
o.Master == param));

app.MapGet("/{num}", (int num) => repo.Find(o => o.Number == num));

app.MapGet("/countCompl", () =>
{
    int count = 0;
    foreach (var o in repo)
    {
        if (o.Status == "Заврешено")
        {
            count++;
        }
    }
    return count;
});

app.MapPut("/{num}", (int num, OrderUpdateDTO dto) =>
{
    Order buffer = repo.Find(o => o.Number == num);
    if (buffer == null)
        return Results.NotFound("ТАКОГО НЕТ");
    if (buffer.Status != dto.Status)
    {
        buffer.Status = dto.Status;
        isUpdatedStatus = true;
        message += "Статус заявки номер " + buffer.Number + " изменен\n";
    }
    buffer.Master = dto.Master;
    buffer.DescriptionProblem = dto.Description;
    if (dto.Comment != null || dto.Comment != "")
    {
        buffer.Comments.Add(dto.Comment);
    }
    return Results.Json(buffer);
});

app.MapGet("/stat/problemTypes", () =>
{
    Dictionary<string, int> result = [];
    foreach (var o in repo)
    {
        if (result.ContainsKey(o.DescriptionProblem))
        {
            result[o.DescriptionProblem]++;
        }
        else
            result[o.DescriptionProblem] = 1;
    }
    return result;
});



app.Run();

record class OrderUpdateDTO(string Status, string Description, string Master, string Comment);

record class OrderGetWithMessage(List<Order> repo, string message);

class Order
{
    int number;
    string startDate;
    string typeTechnic;
    string model;
    string descriptionProblem;
    string client;
    string numberPhone;
    string status;
    string master;

    public Order(int number, string startDate, string typeTechnic, string model, string descriptionProblem, string client, string numberPhone, string status, string master)
    {
        Number = number;
        StartDate = startDate;
        TypeTechnic = typeTechnic;
        Model = model;
        DescriptionProblem = descriptionProblem;
        Client = client;
        NumberPhone = numberPhone;
        Status = status;
        Master = master;
    }

    public int Number { get => number; set => number = value; }
    public string StartDate { get => startDate; set => startDate = value; }
    public string TypeTechnic { get => typeTechnic; set => typeTechnic = value; }
    public string Model { get => model; set => model = value; }
    public string DescriptionProblem { get => descriptionProblem; set => descriptionProblem = value; }
    public string Client { get => client; set => client = value; }
    public string NumberPhone { get => numberPhone; set => numberPhone = value; }
    public string Status { get => status; set => status = value; }
    public string Master { get => master; set => master = value; }
    public List<string> Comments { get; set; } = [];
}