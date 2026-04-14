using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using HMS.Services;
using HMS.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

var app = builder.Build();
app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

// Simple API endpoints that call ClinicService and AuthService. NOTE: This is a demo and not secure.

app.MapPost("/api/login/doctor", (LoginRequest req) =>
{
    if (AuthService.LoginDoctor(req.Identifier)) return Results.Ok(new { role = "Doctor", email = req.Identifier });
    return Results.BadRequest("Doctor not found");
});

app.MapPost("/api/login/student", (LoginRequest req) =>
{
    if (AuthService.LoginStudent(req.Identifier)) return Results.Ok(new { role = "Student", studentId = req.Identifier });
    return Results.BadRequest("Student not found");
});

app.MapPost("/api/logout", () =>
{
    AuthService.Logout();
    return Results.Ok();
});

app.MapGet("/api/patients", () => Results.Ok(ClinicService.Instance.GetPatients()));
app.MapGet("/api/patients/{id}", (int id) => Results.Ok(ClinicService.Instance.GetPatients().FirstOrDefault(p => p.Id == id)));
app.MapPost("/api/patients", (Patient p) =>
{
    try
    {
        var added = ClinicService.Instance.AddPatient(p);
        return Results.Ok(added);
    }
    catch (Exception ex) { return Results.BadRequest(ex.Message); }
});
app.MapPut("/api/patients/{id}", (int id, Patient p) =>
{
    try
    {
        p.Id = id;
        var updated = ClinicService.Instance.UpdatePatient(p);
        return Results.Ok(updated);
    }
    catch (Exception ex) { return Results.BadRequest(ex.Message); }
});
app.MapDelete("/api/patients/{id}", (int id) => Results.Ok(ClinicService.Instance.DeletePatient(id)));

app.MapGet("/api/doctors", () => Results.Ok(ClinicService.Instance.GetDoctors()));
app.MapPost("/api/doctors", (Doctor d) =>
{
    try { return Results.Ok(ClinicService.Instance.AddDoctor(d)); } catch (Exception ex) { return Results.BadRequest(ex.Message); }
});
app.MapPut("/api/doctors/{id}", (int id, Doctor d) =>
{
    try { d.Id = id; return Results.Ok(ClinicService.Instance.UpdateDoctor(d)); } catch (Exception ex) { return Results.BadRequest(ex.Message); }
});
app.MapDelete("/api/doctors/{id}", (int id) => Results.Ok(ClinicService.Instance.DeleteDoctor(id)));

app.MapGet("/api/appointments", () => Results.Ok(ClinicService.Instance.GetAppointments()));
app.MapPost("/api/appointments", (Appointment a) =>
{
    try { return Results.Ok(ClinicService.Instance.AddAppointment(a)); } catch (Exception ex) { return Results.BadRequest(ex.Message); }
});
app.MapPut("/api/appointments/{id}", (int id, Appointment a) =>
{
    try { a.AppointmentId = id; return Results.Ok(ClinicService.Instance.UpdateAppointment(a)); } catch (Exception ex) { return Results.BadRequest(ex.Message); }
});
app.MapDelete("/api/appointments/{id}", (int id) => Results.Ok(ClinicService.Instance.DeleteAppointment(id)));

app.MapGet("/api/bills", () => Results.Ok(ClinicService.Instance.GetBills()));
app.MapPost("/api/bills", (Bill b) =>
{
    try { return Results.Ok(ClinicService.Instance.AddBill(b)); } catch (Exception ex) { return Results.BadRequest(ex.Message); }
});
app.MapPut("/api/bills/{id}", (int id, Bill b) =>
{
    try { b.BillId = id; return Results.Ok(ClinicService.Instance.UpdateBill(b)); } catch (Exception ex) { return Results.BadRequest(ex.Message); }
});
app.MapDelete("/api/bills/{id}", (int id) => Results.Ok(ClinicService.Instance.DeleteBill(id)));

app.MapGet("/api/illness/doctor/{doctorId}", (int doctorId) => Results.Ok(ClinicService.Instance.GetIllnessRecordsForDoctor(doctorId)));
app.MapPost("/api/illness", (IllnessRecord r) =>
{
    try { return Results.Ok(ClinicService.Instance.AddIllnessRecord(r)); } catch (Exception ex) { return Results.BadRequest(ex.Message); }
});
app.MapPut("/api/illness/{id}", (int id, IllnessRecord r) =>
{
    try { r.Id = id; return Results.Ok(ClinicService.Instance.UpdateIllnessRecord(r)); } catch (Exception ex) { return Results.BadRequest(ex.Message); }
});
app.MapDelete("/api/illness/{id}", (int id) => Results.Ok(ClinicService.Instance.DeleteIllnessRecord(id)));

app.MapGet("/api/stats/doctor/{doctorId}", (int doctorId) => Results.Ok(ClinicService.Instance.GetIllnessRecordsForDoctor(doctorId)
    .GroupBy(x => (x.Diagnosis ?? string.Empty).Trim()).Where(g => !string.IsNullOrEmpty(g.Key)).ToDictionary(g => g.Key, g => g.Count())));

app.Run();

record LoginRequest(string Identifier);
