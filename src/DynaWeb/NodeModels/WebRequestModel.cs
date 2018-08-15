using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Graph.Nodes;
using DynaWeb.Properties;
using ProtoCore.AST.AssociativeAST;
using DynaWeb;

namespace CoreNodeModels.Web
{
    /*
    [NodeName("Web Request")]
    [NodeDescription("WebRequestDescription", typeof(Resources))]
    [NodeCategory(BuiltinNodeCategories.CORE_WEB)]
    [IsDesignScriptCompatible]
    [AlsoKnownAs("DynaWebNodesUI.WebRequest")]
    public class SimpleWebRequest : NodeModel
    {
        public SimpleWebRequest()
        {
            InPortData.Add(new PortData("url", Resources.WebRequestPortDataUrlToolTip));
            OutPortData.Add(new PortData("result", Resources.WebRequestPortDataResultToolTip));
            RegisterAllPorts();

            CanUpdatePeriodically = true;
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            var functionCall = AstFactory.BuildFunctionCall(
                new Func<WebRequest, WebResponse>(WebRequest.Execute),
                inputAstNodes
                );

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionCall) };
        }
    }
    */
}
