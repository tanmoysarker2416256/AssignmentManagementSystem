using Microsoft.AspNetCore.Identity;
using AssignmentSystem.Domain.Entities;

namespace AssignmentSystem.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        
        string[] roles = { "Admin", "Teacher", "Student" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }




        if (!context.Classes.Any())
        {
            context.Classes.Add(new Class { Name = "Grade 10 - A" });
            await context.SaveChangesAsync();
        }

        if (!context.Subjects.Any())
        {
            var anyClass = context.Classes.First(); 
            context.Subjects.Add(new Subject { Name = "Mathematics", ClassId = anyClass.Id });
            await context.SaveChangesAsync();
        }

        var classId = context.Classes.First().Id;
        var subjectId = context.Subjects.First().Id;


        
        if (await userManager.FindByEmailAsync("admin@demo.com") == null)
        {
            var admin = new ApplicationUser
            {
                UserName = "admin@demo.com",
                Email = "admin@demo.com",
                FullName = "System Admin",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Admin@123");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

       
        ApplicationUser teacher;
        var existingTeacher = await userManager.FindByEmailAsync("teacher@demo.com");
        if (existingTeacher == null)
        {
            teacher = new ApplicationUser
            {
                UserName = "teacher@demo.com",
                Email = "teacher@demo.com",
                FullName = "Demo Teacher",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(teacher, "Teacher@123");
            await userManager.AddToRoleAsync(teacher, "Teacher");
        }
        else
        {
            teacher = existingTeacher;
        }



       

        if (!context.TeacherSubjects.Any(ts => ts.TeacherId == teacher.Id && ts.SubjectId == subjectId))
        {
            context.TeacherSubjects.Add(new TeacherSubject { TeacherId = teacher.Id, SubjectId = subjectId });
            await context.SaveChangesAsync();
        }



       
        var existingStudent = await userManager.FindByEmailAsync("student@demo.com");
        if (existingStudent == null)
        {
            var student = new ApplicationUser
            {
                UserName = "student@demo.com",
                Email = "student@demo.com",
                FullName = "Demo Student",
                EmailConfirmed = true,
                ClassId = classId
            };
            await userManager.CreateAsync(student, "Student@123");
            await userManager.AddToRoleAsync(student, "Student");
        }
        else if (existingStudent.ClassId == null)
        {
          
            existingStudent.ClassId = classId;
            await userManager.UpdateAsync(existingStudent);
        }
    }
}