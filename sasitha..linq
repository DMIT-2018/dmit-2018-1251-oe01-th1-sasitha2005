<Query Kind="Statements">
  <Connection>
    <ID>bc9c3d12-6db3-4887-bbea-0e5fee65e21e</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>SASITHALAPTOP\SQLEXPRESS01</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>StartTed-2025-Sept</Database>
    <DriverData>
      <LegacyMFA>false</LegacyMFA>
    </DriverData>
  </Connection>
  <NuGetReference>Microsoft.Office.Interop.Excel</NuGetReference>
  <NuGetReference>Microsoft.Office.Interop.Outlook</NuGetReference>
  <NuGetReference>Microsoft.Office.Interop.PowerPoint</NuGetReference>
  <NuGetReference>Microsoft.Office.Interop.Word</NuGetReference>
  <Namespace>Excel = Microsoft.Office.Interop.Excel</Namespace>
  <Namespace>Outlook = Microsoft.Office.Interop.Outlook</Namespace>
  <Namespace>PowerPoint = Microsoft.Office.Interop.PowerPoint</Namespace>
  <Namespace>Word = Microsoft.Office.Interop.Word</Namespace>
  <IncludePredicateBuilder>true</IncludePredicateBuilder>
  <IncludeLinqToSql>true</IncludeLinqToSql>
  <DisableMyExtensions>true</DisableMyExtensions>
  <RuntimeVersion>8.0</RuntimeVersion>
  <TransactionIsolationLevel>ReadCommitted</TransactionIsolationLevel>
</Query>

//Q1 Story:
//As the Student Life Analyst at NAIT, you've been asked by the Dean of Student Affairs to help promote engagement in all student clubs-especially those holding events off the beaten path. The Dean wants a clear, chronological lineup of every upcoming club activity (starting January 1, 2025) that takes place somewhere other than the standard "Scheduled Room," and isn't just the routine BTech Club meeting. This will feed into the new "Get Out & Get Involved" social-media campaign and guide students toward discovering fresh experiences on and off campus.
//Requirements:
//The report should include only those club activities scheduled on or after January 1, 2025, omitting any whose campus venue is labeled "Scheduled Room" or whose name is "BTech Club Meeting." For each qualifying event, it must list the event's start date, the venue name, the hosting club's name, and the activity title, and then present all entries in ascending order by start date.
var getInvolvedEvents = ClubActivities
    .Where(ca => ca.StartDate >= new DateTime(2025, 01, 01)
              && ca.Name != "BTech Club Meeting"
              && ca.CampusVenue.Location != "Scheduled Room")
    .Select(ca => new {
        StartDate = ca.StartDate,
        Location = ca.CampusVenue.Location,
        Club = ca.Club.ClubName,
        Activity = ca.Name
    })
    .OrderBy(ca => ca.StartDate);

getInvolvedEvents.Dump("Get out & Get Involved");

 
 //Question 2 
 //Question 2 (2 Marks)
//Story:
//As the Academic Planning Analyst at NAIT, you've been asked to generate a comprehensive overview of every program offered across our schools that meets accreditation requirements. Your report will translate each school code into a friendly name, tally how many courses in each program are mandatory versus optional, and then highlight only those programs that have at least twenty-two required courses, so department heads can prioritize resourcing and scheduling decisions.
//Requirements:
//You must map SchoolCode to its full school name ("SAMIT" → "School of Advance Media and IT", "SEET" → "School of Electrical Engineering Technology", all others → "Unknown"), include each program name, count the number of required courses and optional courses, filter to only those with required course count greater than or equal to 22, and order the final list by program name in ascending order.
var ProgramCoursesSchoolEvents =Programs.Where(p=> ProgramCourses.Count(pc=> pc.ProgramID == p.ProgramID && pc.Required) >= 22).Select(p=> new {School = p.SchoolCode == "SAMIT" ? "School of Advanced Media And IT" : p.SchoolCode == "SEET" ? "School of Electrical Engineering" : "Unknown", ProgramName = p.ProgramName, RequiredCount = ProgramCourses.Count(pc=> pc.ProgramID == p.ProgramID && pc.Required), OptionalCount = ProgramCourses.Count(pc=> pc.ProgramID == p.ProgramID && !pc.Required)}).OrderBy(p=> p.ProgramName);
ProgramCoursesSchoolEvents.Dump("Program Courses 22 + Requirements");

//Question 3 
//Question 3 (2 Marks)
//Story:
//As the International Student Services Officer, you need to identify all non-Canadian students who have not yet made any tuition payments. This allows your team to reach out proactively and assist them in completing their registration. You'll also provide each student's home country and indicate whether they've joined any campus clubs to gauge their level of campus engagement.

//Requirements:
//You must filter Students to those with zero entries in StudentPayments and country is not from "Canada", then order by last name ascending. For each student, report their Student Number, the full country name, their full name, and a club membership count that displays "None" if they belong to no clubs or the actual number of club memberships otherwise.
var ThirdResult = Students.Where(s=> !StudentPayments.Any(sp=> sp.StudentNumber == s.StudentNumber) && s.Countries.CountryName != "Canada" ).OrderBy(s=> s.LastName).Select(s=> new{ StudentNumber = s.StudentNumber, CountryName = s.Countries.CountryName, FullName = s.FirstName + " " + s.LastName, ClubMemberships = ClubMembers.Count(cm=> cm.StudentNumber == s.StudentNumber) == 0 ? "None" : ClubMembers.Count(cm=> cm.StudentNumber == s.StudentNumber).ToString()});
ThirdResult.Dump("Non-Canadian Students With No Payments");
//Question 4 
//Question 4 (2 Marks)
//Story:
//As the Department Chair, you want to review all active instructors currently teaching classes this term. You need a ranked list showing each instructor's program affiliation, their full name, and a simple workload category-so you can balance teaching assignments and offer support where needed.
//Requirements:
//You must select employees those whose position is an "Instructor", the release date is null, and who have taught at least one class in ClassOfferings, then order first by descending number of class offerig and then by LastName. For each instructor, report their program name, full name, and a WorkLoad label of "High" if they teach more than 24 offerings, "Med" if more than 8, or "Low" otherwise.
var results = Employees.Where(e => e.Position.Description == "Instructor" && e.ReleaseDate == null && ClassOfferings.Any(co => co.EmployeeID == e.EmployeeID)).OrderByDescending(e=> ClassOfferings.Count(co => co.EmployeeID == e.EmployeeID)).ThenBy(e=> e.LastName).Select(e=> new{ ProgramName = e.Program.ProgramName, FullName = e.FirstName + " " + e.LastName, WorkLoadLabel = ClassOfferings.Count(co=> co.EmployeeID == e.EmployeeID) > 24 ? "High" : ClassOfferings.Count(co=> co.EmployeeID == e.EmployeeID) > 8 ? "Med" : "Low"});
results.Dump("Instructor Workflow Overload"); 

//Question 5
//Question 5 (2 Marks)
//Story:
//As the Vice-President of Student Clubs, you've been tasked with producing a snapshot of all clubs on campus. Your report will list each club's faculty supervisor, membership size, and upcoming activity count-so you can recognize high-performing clubs and identify those that may need support.
//Requirements:
//Your report must contain supervisor (use "Unknown" if Employee is null, otherwise full name), club name, member count (the number of entries in ClubMembers table), and Activities (display "None Schedule" if ClubActivities.Count() == 0, otherwise the count). Finally, order the list by member count in descending order.
var nextResults = Clubs.Select(c=> new{ Supervisor =  c.Employee == null ? "Unknown" : c.Employee.FirstName + " " + c.Employee.LastName, Club = c.ClubName, MemberCount = ClubMembers.Count(cm=> cm.ClubID == c.ClubID), Activities = ClubActivities.Count(ca => ca.ClubID == c.ClubID) == 0 ? "None Scheduled" : ClubActivities.Count(ca=> ca.ClubID == c.ClubID).ToString()}).OrderByDescending(c=> c.MemberCount);
nextResults.Dump("Club Members List");