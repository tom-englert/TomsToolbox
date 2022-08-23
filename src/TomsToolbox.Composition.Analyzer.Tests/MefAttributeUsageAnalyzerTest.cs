using VerifyCS = CSharpAnalyzerVerifier<MefAttributeUsageAnalyzer>;

public class Mef1AttributeUsageAnalyzerTest
{
    [Fact]
    public async Task NoDiagnosticsOnEmptySource()
    {
        var test = @"";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task ErrorOnExportedTypeWithoutCreationPolicyAttribute()
    {
        var test = @"
    using System;
    using System.ComponentModel.Composition;

    namespace ConsoleApplication1
    {
        [Export]
        class {|#0:TypeName|}
        {   
        }
    }";

        var expected = VerifyCS.Diagnostic(ExtensionMethods.NoCreationPolicyRuleMef1.Id).WithLocation(0).WithArguments("TypeName");
        
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task NoErrorOnExportedTypeWithCreationPolicyAttribute()
    {
        var test = @"
    using System;
    using System.ComponentModel.Composition;

    namespace ConsoleApplication1
    {
        [Export]
        [PartCreationPolicy(CreationPolicy.Shared)]
        class {|#0:TypeName|}
        {   
        }
    }";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task ErrorOnTypeWithImportAttribute()
    {
        var test = @"
    using System;
    using System.ComponentModel.Composition;

    namespace ConsoleApplication1
    {
        [Export]
        [PartCreationPolicy(CreationPolicy.Shared)]
        class {|#0:TypeName|}
        {   
            [ImportingConstructor]
            TypeName(){}

            [{|#1:Import|}]
            int Property1 { get; set; }

            [{|#2:ImportMany|}(""Test"")]
            int Property2 { get; set; }
        }
    }";

        var expected0 = VerifyCS.Diagnostic(ExtensionMethods.NoPublicConstructorRule.Id).WithLocation(0);
        var expected1 = VerifyCS.Diagnostic(ExtensionMethods.AvoidImportAttributesRule.Id).WithLocation(1).WithArguments("Import");
        var expected2 = VerifyCS.Diagnostic(ExtensionMethods.AvoidImportAttributesRule.Id).WithLocation(2).WithArguments("ImportMany");

        await VerifyCS.VerifyAnalyzerAsync(test, expected0, expected1, expected2);
    }

    [Fact]
    public async Task ErrorOnTypeWithSuspiciousCreationPolicy()
    {
        var test = @"
    using System;
    using System.ComponentModel.Composition;

    namespace System.Web.Http {
        class ApiController { }
    }

    namespace ConsoleApplication1
    {
        class CustomController : System.Web.Http.ApiController {}


        [Export]
        [PartCreationPolicy(CreationPolicy.Shared)]
        class {|#0:TypeName0|} : CustomController
        {   
        }

#pragma warning disable MEF001
        [Export]
        class {|#1:TypeName1|} : CustomController
        {   
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        class {|#2:TypeName2|} : CustomController
        {   
        }
    }";

        var expected1 = VerifyCS.Diagnostic(ExtensionMethods.SuspiciousPolicyRule.Id).WithLocation(0).WithArguments("ConsoleApplication1.TypeName0 : ConsoleApplication1.CustomController : System.Web.Http.ApiController : System.Object");
        var expected2 = VerifyCS.Diagnostic(ExtensionMethods.SuspiciousPolicyRule.Id).WithLocation(1).WithArguments("ConsoleApplication1.TypeName1 : ConsoleApplication1.CustomController : System.Web.Http.ApiController : System.Object");

        await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
    }

    [Fact]
    public async Task ErrorOnTypeWithoutImportingConstructor()
    {
        var test = @"
    using System;
    using System.ComponentModel.Composition;

    namespace ConsoleApplication1
    {
        [Export]
        [PartCreationPolicy(CreationPolicy.Shared)]
        class {|#0:TypeName|}
        {   
            TypeName(int param1){}
        }
    }";

        var expected0 = VerifyCS.Diagnostic(ExtensionMethods.NoPublicConstructorRule.Id).WithLocation(0);
        var expected1 = VerifyCS.Diagnostic(ExtensionMethods.NoImportingConstructorRule.Id).WithLocation(0);

        await VerifyCS.VerifyAnalyzerAsync(test, expected0, expected1);
    }
}

public class Mef2AttributeUsageAnalyzerTest
{
    [Fact]
    public async Task ErrorOnExportedTypeWithoutCreationPolicyAttribute()
    {
        var test = @"
    using System;
    using System.Composition;

    namespace ConsoleApplication1
    {
        [Export]
        class {|#0:TypeName|}
        {   
        }
    }";

        var expected = VerifyCS.Diagnostic(ExtensionMethods.NoCreationPolicyRuleMef2.Id).WithLocation(0).WithArguments("TypeName");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task NoErrorOnExportedTypeWithCreationPolicyAttribute()
    {
        var test = @"
    using System;
    using System.Composition;

    namespace ConsoleApplication1
    {
        [Export]
        [Shared]
        class {|#0:TypeName|}
        {   
        }
    }";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task ErrorOnTypeWithImportAttribute()
    {
        var test = @"
    using System;
    using System.Composition;

    namespace ConsoleApplication1
    {
        [Export]
        [Shared]
        class TypeName
        {   
            [ImportingConstructor]
            public TypeName(){}

            [{|#0:Import|}]
            int Property1 { get; set; }

            [{|#1:ImportMany|}(""Test"")]
            int Property2 { get; set; }
        }
    }";

        var expected1 = VerifyCS.Diagnostic(ExtensionMethods.AvoidImportAttributesRule.Id).WithLocation(0).WithArguments("Import");
        var expected2 = VerifyCS.Diagnostic(ExtensionMethods.AvoidImportAttributesRule.Id).WithLocation(1).WithArguments("ImportMany");

        await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
    }

    [Fact]
    public async Task ErrorOnTypeWithSuspiciousCreationPolicy()
    {
        var test = @"
    using System;
    using System.Composition;

    namespace System.Web.Http {
        class ApiController { }
    }

    namespace System.Composition 
    {
        // supplied by the code generator
        class NonSharedAttribute : Attribute {}
    }   

    namespace ConsoleApplication1
    {
        class CustomController : System.Web.Http.ApiController {}

        [Export]
        [Shared]
        class {|#0:TypeName0|} : CustomController
        {   
        }

#pragma warning disable MEF004
        [Export]
        class {|#1:TypeName1|} : CustomController
        {   
        }

        [Export]
        [NonShared]
        class {|#2:TypeName2|} : CustomController
        {   
        }

        [Export, Shared]
        [NonShared]
        class {|#3:TypeName3|} : CustomController
        {   
        }
    }";

        var expected1 = VerifyCS.Diagnostic(ExtensionMethods.SuspiciousPolicyRule.Id).WithLocation(0).WithArguments("ConsoleApplication1.TypeName0 : ConsoleApplication1.CustomController : System.Web.Http.ApiController : System.Object");
        var expected2 = VerifyCS.Diagnostic(ExtensionMethods.MultipleCreationPolicyRule.Id).WithLocation(3);

        await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
    }
}
