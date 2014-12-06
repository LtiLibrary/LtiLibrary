using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Provider.Migrations
{
    using Models;
    using System.Data.Entity.Migrations;

    public sealed class Configuration : DbMigrationsConfiguration<ProviderContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ProviderContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!roleManager.RoleExists(UserRoles.AdminRole)) roleManager.Create(new IdentityRole(UserRoles.AdminRole));
            if (!roleManager.RoleExists(UserRoles.SuperUserRole)) roleManager.Create(new IdentityRole(UserRoles.SuperUserRole));

            context.Consumers.AddOrUpdate(new [] {
                new Consumer { 
                    ConsumerId = 1, 
                    Name = "LTI Tool Consumer", 
                    Key = "1e87e688459b45e4", 
                    Secret = "2aac4af7a1ac4ae7"
                },
                new Consumer {
                    ConsumerId = 2,
                    Name = "Localhost Tool Consumer",
                    Key = "0e87e688459b45e4",
                    Secret = "2aac4af7a1ac4ae7"
                }
            });
            context.Tools.AddOrUpdate(new[] {
                new Tool {
                    ToolId = 1,
                    Name = "Emancipation Proclamation",
                    Description = "Abraham Lincoln's Emancipation Proclamation",
                    Content = @"<p><span class=""heading"">The Emancipation Proclamation</span><br /> <strong>January 1, 1863</strong></p>
                        <p><strong>A Transcription</strong></p>
                        <p>By the President of the United States of America:</p>
                        <p>A Proclamation.</p>
                        <p>Whereas, on the twenty-second day of September, in the year of our Lord one thousand eight hundred and 
                        sixty-two, a proclamation was issued by the President of the United States, containing, among other things, 
                        the following, to wit:</p>
                        <p>""That on the first day of January, in the year of our Lord one thousand eight hundred and sixty-three, 
                        all persons held as slaves within any State or designated part of a State, the people whereof shall then be 
                        in rebellion against the United States, shall be then, thenceforward, and forever free; and the Executive 
                        Government of the United States, including the military and naval authority thereof, will recognize and 
                        maintain the freedom of such persons, and will do no act or acts to repress such persons, or any of them, 
                        in any efforts they may make for their actual freedom.</p>
                        <p>""That the Executive will, on the first day of January aforesaid, by proclamation, designate the States 
                        and parts of States, if any, in which the people thereof, respectively, shall then be in rebellion against 
                        the United States; and the fact that any State, or the people thereof, shall on that day be, in good faith, 
                        represented in the Congress of the United States by members chosen thereto at elections wherein a majority 
                        of the qualified voters of such State shall have participated, shall, in the absence of strong countervailing 
                        testimony, be deemed conclusive evidence that such State, and the people thereof, are not then in rebellion 
                        against the United States.""</p>
                        <p>Now, therefore I, Abraham Lincoln, President of the United States, by virtue of the power in me vested as 
                        Commander-in-Chief, of the Army and Navy of the United States in time of actual armed rebellion against the 
                        authority and government of the United States, and as a fit and necessary war measure for suppressing said 
                        rebellion, do, on this first day of January, in the year of our Lord one thousand eight hundred and 
                        sixty-three, and in accordance with my purpose so to do publicly proclaimed for the full period of one 
                        hundred days, from the day first above mentioned, order and designate as the States and parts of States 
                        wherein the people thereof respectively, are this day in rebellion against the United States, the following, 
                        to wit:</p>
                        <p>Arkansas, Texas, Louisiana, (except the Parishes of St. Bernard, Plaquemines, Jefferson, St. John, 
                        St. Charles, St. James Ascension, Assumption, Terrebonne, Lafourche, St. Mary, St. Martin, and Orleans, 
                        including the City of New Orleans) Mississippi, Alabama, Florida, Georgia, South Carolina, North Carolina, 
                        and Virginia, (except the forty-eight counties designated as West Virginia, and also the counties of Berkley, 
                        Accomac, Northampton, Elizabeth City, York, Princess Ann, and Norfolk, including the cities of Norfolk and 
                        Portsmouth[)], and which excepted parts, are for the present, left precisely as if this proclamation were not 
                        issued.</p>
                        <p>And by virtue of the power, and for the purpose aforesaid, I do order and declare that all persons held as 
                        slaves within said designated States, and parts of States, are, and henceforward shall be free; and that the 
                        Executive government of the United States, including the military and naval authorities thereof, will recognize 
                        and maintain the freedom of said persons.</p>
                        <p>And I hereby enjoin upon the people so declared to be free to abstain from all violence, unless in necessary 
                        self-defence; and I recommend to them that, in all cases when allowed, they labor faithfully for reasonable 
                        wages.</p>
                        <p>And I further declare and make known, that such persons of suitable condition, will be received into the 
                        armed service of the United States to garrison forts, positions, stations, and other places, and to man vessels 
                        of all sorts in said service.</p>
                        <p>And upon this act, sincerely believed to be an act of justice, warranted by the Constitution, upon military 
                        necessity, I invoke the considerate judgment of mankind, and the gracious favor of Almighty God.</p>
                        <p>In witness whereof, I have hereunto set my hand and caused the seal of the United States to be affixed.</p>
                        <p>Done at the City of Washington, this first day of January, in the year of our Lord one thousand eight hundred 
                        and sixty three, and of the Independence of the United States of America the eighty-seventh.</p>
                        <p>By the President: ABRAHAM LINCOLN <br /> WILLIAM H. SEWARD, Secretary of State.</p>"
                },
                new Tool {
                    ToolId = 2,
                    Name = "Gettysburg Address",
                    Description = "Abraham Lincoln's Gettysburg Address",
                    Content = @"<p><strong>The Gettysburg Address</strong> <br /> <br />Four score and seven years ago our
                        fathers brought forth on this continent, a new nation, conceived in Liberty, and dedicated to the 
                        proposition that all men are created equal. <br /> <br />Now we are engaged in a great civil war, 
                        testing whether that nation, or any nation so conceived and so dedicated, can long endure. We are 
                        met on a great battlefield of that war. We have come to dedicate a portion of that field, as a final 
                        resting place for those who here gave their lives that that nation might live. It is altogether fitting 
                        and proper that we should do this. <br /> <br />But, in a larger sense, we cannot dedicate&mdash;we 
                        cannot consecrate&mdash;we cannot hallow&mdash;this ground. The brave men, living and dead, who 
                        struggled here, have consecrated it, far above our poor power to add or detract. The world will little 
                        note, nor long remember what we say here, but it can never forget what they did here. It is for us the 
                        living, rather, to be dedicated here to the unfinished work which they who fought here have thus far 
                        so nobly advanced. It is rather for us to be here dedicated to the great task remaining before 
                        us&mdash;that from these honored dead we take increased devotion to that cause for which they gave the 
                        last full measure of devotion&mdash;that we here highly resolve that these dead shall not have died in 
                        vain&mdash;that this nation, under God, shall have a new birth of freedom&mdash; and that government of 
                        the people, by the people, for the people, shall not perish from the earth.</p>"
                },
                new Tool {
                    ToolId = 3,
                    Name = "Sample Tool",
                    Description = "This is a sample tool.",
                    Content = @"<h1>Sample Tool</h1><p>This is a sample tool.</p>"
                }            
            });
        }
    }
}
