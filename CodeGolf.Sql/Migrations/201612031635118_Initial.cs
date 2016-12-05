namespace CodeGolf.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Problems",
                c => new
                    {
                        ProblemId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Language = c.String(),
                        AuthorId = c.Int(),
                        AnyLanguage = c.Boolean(nullable: false),
                        EnforceOutput = c.Boolean(nullable: false),
                        Closed = c.Boolean(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ProblemId)
                .ForeignKey("dbo.Users", t => t.AuthorId)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Identity = c.String(),
                        Authentication = c.String(),
                        Score = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.SolutionComments",
                c => new
                    {
                        SolutionCommentId = c.Int(nullable: false, identity: true),
                        Comment = c.String(),
                        CommentorId = c.Int(),
                        SolutionId = c.Int(),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SolutionCommentId)
                .ForeignKey("dbo.Users", t => t.CommentorId)
                .ForeignKey("dbo.Solutions", t => t.SolutionId)
                .Index(t => t.CommentorId)
                .Index(t => t.SolutionId);
            
            CreateTable(
                "dbo.Solutions",
                c => new
                    {
                        SolutionId = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        Language = c.String(),
                        ProblemId = c.Int(),
                        AuthorId = c.Int(),
                        Passing = c.Boolean(),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SolutionId)
                .ForeignKey("dbo.Users", t => t.AuthorId)
                .ForeignKey("dbo.Problems", t => t.ProblemId)
                .Index(t => t.ProblemId)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.Votes",
                c => new
                    {
                        VoteId = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(nullable: false),
                        VoterId = c.Int(),
                        Value = c.Int(nullable: false),
                        Solution_SolutionId = c.Int(),
                    })
                .PrimaryKey(t => t.VoteId)
                .ForeignKey("dbo.Users", t => t.VoterId)
                .ForeignKey("dbo.Solutions", t => t.Solution_SolutionId)
                .Index(t => t.VoterId)
                .Index(t => t.Solution_SolutionId);
            
            CreateTable(
                "dbo.TestCases",
                c => new
                    {
                        TestCaseId = c.Int(nullable: false, identity: true),
                        ProblemId = c.Int(),
                        Input = c.String(),
                        Output = c.String(),
                    })
                .PrimaryKey(t => t.TestCaseId)
                .ForeignKey("dbo.Problems", t => t.ProblemId)
                .Index(t => t.ProblemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TestCases", "ProblemId", "dbo.Problems");
            DropForeignKey("dbo.Problems", "AuthorId", "dbo.Users");
            DropForeignKey("dbo.SolutionComments", "SolutionId", "dbo.Solutions");
            DropForeignKey("dbo.Votes", "Solution_SolutionId", "dbo.Solutions");
            DropForeignKey("dbo.Votes", "VoterId", "dbo.Users");
            DropForeignKey("dbo.Solutions", "ProblemId", "dbo.Problems");
            DropForeignKey("dbo.Solutions", "AuthorId", "dbo.Users");
            DropForeignKey("dbo.SolutionComments", "CommentorId", "dbo.Users");
            DropIndex("dbo.TestCases", new[] { "ProblemId" });
            DropIndex("dbo.Votes", new[] { "Solution_SolutionId" });
            DropIndex("dbo.Votes", new[] { "VoterId" });
            DropIndex("dbo.Solutions", new[] { "AuthorId" });
            DropIndex("dbo.Solutions", new[] { "ProblemId" });
            DropIndex("dbo.SolutionComments", new[] { "SolutionId" });
            DropIndex("dbo.SolutionComments", new[] { "CommentorId" });
            DropIndex("dbo.Problems", new[] { "AuthorId" });
            DropTable("dbo.TestCases");
            DropTable("dbo.Votes");
            DropTable("dbo.Solutions");
            DropTable("dbo.SolutionComments");
            DropTable("dbo.Users");
            DropTable("dbo.Problems");
        }
    }
}
