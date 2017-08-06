using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Graph.Nodes;
using DynWWW.Properties;
using ProtoCore.AST.AssociativeAST;

namespace DynWWW.NodeModels
{
    [NodeName("Web Request")]
    [NodeDescription("WebRequestDescription", typeof(Resources))]
    [NodeCategory(BuiltinNodeCategories.CORE_WEB)]
    [IsDesignScriptCompatible]
    [AlsoKnownAs("DSCoreNodesUI.WebRequest")]
    public class WebRequest : NodeModel
    {
        public WebRequest()
        {
            InPortData.Add(new PortData("url", Resources.WebRequestPortDataUrlToolTip));
            OutPortData.Add(new PortData("result", Resources.WebRequestPortDataResultToolTip));
            RegisterAllPorts();

            CanUpdatePeriodically = true;
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            var functionCall = AstFactory.BuildFunctionCall(new Func<string, string>(Request.WebRequestByUrl), inputAstNodes);

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionCall) };
        }
    }
}
