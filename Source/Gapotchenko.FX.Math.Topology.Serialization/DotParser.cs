// This code was generated by the Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, John Gough, QUT 2005-2014
// (see accompanying GPPGcopyright.rtf)

// Input file <Dot.y - 26.11.2021 21:58:04>

// options: no-lines

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Text;
using Gapotchenko.FX.Math.Topology.Serialization.ParserToolkit;

namespace Gapotchenko.FX.Math.Topology.Serialization
{
internal enum DotTokens {
    error=127,EOF=128,DIGRAPH=129,GRAPH=130,ARROW=131,SUBGRAPH=132,
    NODE=133,EDGE=134,ID=135};

internal partial struct DotValueType
{
    public string? sVal;
    public KeyValuePair<string, string> avPair;
    public DotParser.Cell<DotDocumentVertex> sList;
	public DotParser.Cell<DotParser.Cell<DotDocumentVertex>> sLists;
    public List<KeyValuePair<string, string>>? attributes;
    public DotDocumentVertex vertex;
}
[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
internal partial class DotParser: ShiftReduceParser<DotValueType, LexLocation>
{
#pragma warning disable 649
  private static Dictionary<int, string>? aliases;
#pragma warning restore 649
  private static Rule[] rules = new Rule[51];
  private static State[] states = new State[83];
  private static string[] nonTerms = new string[] {
      "graph", "$accept", "graphType", "graphName", "stmt_list", "stmt", "node_stmt", 
      "edge_stmt", "attr_stmt", "subgraph", "node_id", "opt_attr_list", "endpoint", 
      "edgeRHS", "id", "Anon@1", "attr_list", "a_list", "avPair", "opt_port", 
      "port", "port_location", "port_angle", };

  static DotParser() {
    states[0] = new State(new int[]{129,78,130,79,135,80},new int[]{-1,1,-3,3});
    states[1] = new State(new int[]{128,2});
    states[2] = new State(-1);
    states[3] = new State(new int[]{135,77,123,-8},new int[]{-4,4});
    states[4] = new State(new int[]{123,5});
    states[5] = new State(new int[]{135,12,123,55,132,59,130,64,133,66,134,68,125,-9},new int[]{-5,6,-6,8,-7,15,-11,16,-15,31,-8,47,-13,48,-10,58,-9,63});
    states[6] = new State(new int[]{125,7});
    states[7] = new State(-2);
    states[8] = new State(new int[]{59,10,135,12,123,55,132,59,130,64,133,66,134,68,125,-9},new int[]{-5,9,-6,8,-7,15,-11,16,-15,31,-8,47,-13,48,-10,58,-9,63});
    states[9] = new State(-10);
    states[10] = new State(new int[]{135,12,123,55,132,59,130,64,133,66,134,68,125,-9},new int[]{-5,11,-6,8,-7,15,-11,16,-15,31,-8,47,-13,48,-10,58,-9,63});
    states[11] = new State(-11);
    states[12] = new State(new int[]{61,13,58,-49,64,-49,91,-49,59,-49,135,-49,123,-49,132,-49,130,-49,133,-49,134,-49,125,-49,131,-49});
    states[13] = new State(new int[]{135,14});
    states[14] = new State(-12);
    states[15] = new State(-13);
    states[16] = new State(new int[]{91,19,131,-19,59,-31,135,-31,123,-31,132,-31,130,-31,133,-31,134,-31,125,-31},new int[]{-12,17,-17,18});
    states[17] = new State(-17);
    states[18] = new State(-32);
    states[19] = new State(new int[]{93,20,135,30},new int[]{-18,21,-19,23,-15,27});
    states[20] = new State(-33);
    states[21] = new State(new int[]{93,22});
    states[22] = new State(-34);
    states[23] = new State(new int[]{44,25,135,30,93,-35},new int[]{-18,24,-19,23,-15,27});
    states[24] = new State(-36);
    states[25] = new State(new int[]{135,30},new int[]{-18,26,-19,23,-15,27});
    states[26] = new State(-37);
    states[27] = new State(new int[]{61,28,44,-39,135,-39,93,-39});
    states[28] = new State(new int[]{135,30},new int[]{-15,29});
    states[29] = new State(-38);
    states[30] = new State(-49);
    states[31] = new State(new int[]{58,38,64,36,91,-41,59,-41,135,-41,123,-41,132,-41,130,-41,133,-41,134,-41,125,-41,131,-41},new int[]{-20,32,-21,33,-22,34,-23,45});
    states[32] = new State(-40);
    states[33] = new State(-42);
    states[34] = new State(new int[]{64,36,91,-43,59,-43,135,-43,123,-43,132,-43,130,-43,133,-43,134,-43,125,-43,131,-43},new int[]{-23,35});
    states[35] = new State(-46);
    states[36] = new State(new int[]{135,30},new int[]{-15,37});
    states[37] = new State(-50);
    states[38] = new State(new int[]{40,40,135,30},new int[]{-15,39});
    states[39] = new State(-47);
    states[40] = new State(new int[]{135,30},new int[]{-15,41});
    states[41] = new State(new int[]{44,42});
    states[42] = new State(new int[]{135,30},new int[]{-15,43});
    states[43] = new State(new int[]{41,44});
    states[44] = new State(-48);
    states[45] = new State(new int[]{58,38,91,-44,59,-44,135,-44,123,-44,132,-44,130,-44,133,-44,134,-44,125,-44,131,-44},new int[]{-22,46});
    states[46] = new State(-45);
    states[47] = new State(-14);
    states[48] = new State(new int[]{131,75},new int[]{-14,49});
    states[49] = new State(new int[]{131,51,91,19,59,-31,135,-31,123,-31,132,-31,130,-31,133,-31,134,-31,125,-31},new int[]{-12,50,-17,18});
    states[50] = new State(-18);
    states[51] = new State(new int[]{135,30,123,55,132,59},new int[]{-13,52,-11,53,-15,31,-10,54});
    states[52] = new State(-22);
    states[53] = new State(-19);
    states[54] = new State(-20);
    states[55] = new State(new int[]{135,12,123,55,132,59,130,64,133,66,134,68,125,-9},new int[]{-5,56,-6,8,-7,15,-11,16,-15,31,-8,47,-13,48,-10,58,-9,63});
    states[56] = new State(new int[]{125,57});
    states[57] = new State(-23);
    states[58] = new State(new int[]{131,-20,59,-16,135,-16,123,-16,132,-16,130,-16,133,-16,134,-16,125,-16});
    states[59] = new State(new int[]{123,60,135,30,59,-27,132,-27,130,-27,133,-27,134,-27,125,-27,131,-27,91,-27},new int[]{-15,70});
    states[60] = new State(new int[]{135,12,123,55,132,59,130,64,133,66,134,68,125,-9},new int[]{-5,61,-6,8,-7,15,-11,16,-15,31,-8,47,-13,48,-10,58,-9,63});
    states[61] = new State(new int[]{125,62});
    states[62] = new State(-24);
    states[63] = new State(-15);
    states[64] = new State(new int[]{91,19},new int[]{-17,65});
    states[65] = new State(-28);
    states[66] = new State(new int[]{91,19},new int[]{-17,67});
    states[67] = new State(-29);
    states[68] = new State(new int[]{91,19},new int[]{-17,69});
    states[69] = new State(-30);
    states[70] = new State(-25,new int[]{-16,71});
    states[71] = new State(new int[]{123,72});
    states[72] = new State(new int[]{135,12,123,55,132,59,130,64,133,66,134,68,125,-9},new int[]{-5,73,-6,8,-7,15,-11,16,-15,31,-8,47,-13,48,-10,58,-9,63});
    states[73] = new State(new int[]{125,74});
    states[74] = new State(-26);
    states[75] = new State(new int[]{135,30,123,55,132,59},new int[]{-13,76,-11,53,-15,31,-10,54});
    states[76] = new State(-21);
    states[77] = new State(-7);
    states[78] = new State(-3);
    states[79] = new State(-4);
    states[80] = new State(new int[]{129,81,130,82});
    states[81] = new State(-5);
    states[82] = new State(-6);

    for (int sNo = 0; sNo < states.Length; sNo++) states[sNo].number = sNo;

    rules[1] = new Rule(-2, new int[]{-1,128});
    rules[2] = new Rule(-1, new int[]{-3,-4,123,-5,125});
    rules[3] = new Rule(-3, new int[]{129});
    rules[4] = new Rule(-3, new int[]{130});
    rules[5] = new Rule(-3, new int[]{135,129});
    rules[6] = new Rule(-3, new int[]{135,130});
    rules[7] = new Rule(-4, new int[]{135});
    rules[8] = new Rule(-4, new int[]{});
    rules[9] = new Rule(-5, new int[]{});
    rules[10] = new Rule(-5, new int[]{-6,-5});
    rules[11] = new Rule(-5, new int[]{-6,59,-5});
    rules[12] = new Rule(-6, new int[]{135,61,135});
    rules[13] = new Rule(-6, new int[]{-7});
    rules[14] = new Rule(-6, new int[]{-8});
    rules[15] = new Rule(-6, new int[]{-9});
    rules[16] = new Rule(-6, new int[]{-10});
    rules[17] = new Rule(-7, new int[]{-11,-12});
    rules[18] = new Rule(-8, new int[]{-13,-14,-12});
    rules[19] = new Rule(-13, new int[]{-11});
    rules[20] = new Rule(-13, new int[]{-10});
    rules[21] = new Rule(-14, new int[]{131,-13});
    rules[22] = new Rule(-14, new int[]{-14,131,-13});
    rules[23] = new Rule(-10, new int[]{123,-5,125});
    rules[24] = new Rule(-10, new int[]{132,123,-5,125});
    rules[25] = new Rule(-16, new int[]{});
    rules[26] = new Rule(-10, new int[]{132,-15,-16,123,-5,125});
    rules[27] = new Rule(-10, new int[]{132});
    rules[28] = new Rule(-9, new int[]{130,-17});
    rules[29] = new Rule(-9, new int[]{133,-17});
    rules[30] = new Rule(-9, new int[]{134,-17});
    rules[31] = new Rule(-12, new int[]{});
    rules[32] = new Rule(-12, new int[]{-17});
    rules[33] = new Rule(-17, new int[]{91,93});
    rules[34] = new Rule(-17, new int[]{91,-18,93});
    rules[35] = new Rule(-18, new int[]{-19});
    rules[36] = new Rule(-18, new int[]{-19,-18});
    rules[37] = new Rule(-18, new int[]{-19,44,-18});
    rules[38] = new Rule(-19, new int[]{-15,61,-15});
    rules[39] = new Rule(-19, new int[]{-15});
    rules[40] = new Rule(-11, new int[]{-15,-20});
    rules[41] = new Rule(-20, new int[]{});
    rules[42] = new Rule(-20, new int[]{-21});
    rules[43] = new Rule(-21, new int[]{-22});
    rules[44] = new Rule(-21, new int[]{-23});
    rules[45] = new Rule(-21, new int[]{-23,-22});
    rules[46] = new Rule(-21, new int[]{-22,-23});
    rules[47] = new Rule(-22, new int[]{58,-15});
    rules[48] = new Rule(-22, new int[]{58,40,-15,44,-15,41});
    rules[49] = new Rule(-15, new int[]{135});
    rules[50] = new Rule(-23, new int[]{64,-15});
  }

  protected override void Initialize() {
    this.InitSpecialTokens((int)DotTokens.error, (int)DotTokens.EOF);
    this.InitStates(states);
    this.InitRules(rules);
    this.InitNonTerminals(nonTerms);
  }

  protected override void DoAction(int action)
  {
#pragma warning disable 162, 1522
    switch (action)
    {
      case 3: // graphType -> DIGRAPH
{ _strict = false; _directed = true; }
        break;
      case 4: // graphType -> GRAPH
{ _strict = false; _directed = false; }
        break;
      case 5: // graphType -> ID, DIGRAPH
{ _strict = ValueStack[ValueStack.Depth-2].sVal != null && "strict" == ValueStack[ValueStack.Depth-2].sVal!.ToLower(); _directed = true; }
        break;
      case 6: // graphType -> ID, GRAPH
{ _strict = ValueStack[ValueStack.Depth-2].sVal != null && "strict" == ValueStack[ValueStack.Depth-2].sVal!.ToLower(); _directed = false; }
        break;
      case 7: // graphName -> ID
{ _graphId = ValueStack[ValueStack.Depth-1].sVal; }
        break;
      case 8: // graphName -> /* empty */
{ }
        break;
      case 9: // stmt_list -> /* empty */
{ CurrentSemanticValue.sList = new(); }
        break;
      case 10: // stmt_list -> stmt, stmt_list
{ CurrentSemanticValue.sList = Append(ValueStack[ValueStack.Depth-2].sList, ValueStack[ValueStack.Depth-1].sList); }
        break;
      case 11: // stmt_list -> stmt, ';', stmt_list
{ CurrentSemanticValue.sList = Append(ValueStack[ValueStack.Depth-3].sList, ValueStack[ValueStack.Depth-1].sList); }
        break;
      case 12: // stmt -> ID, '=', ID
{ MkEqStmt(ValueStack[ValueStack.Depth-3].sVal, ValueStack[ValueStack.Depth-1].sVal); CurrentSemanticValue.sList = new(); }
        break;
      case 13: // stmt -> node_stmt
{ CurrentSemanticValue.sList = ValueStack[ValueStack.Depth-1].sList; }
        break;
      case 14: // stmt -> edge_stmt
{ CurrentSemanticValue.sList = ValueStack[ValueStack.Depth-1].sList; }
        break;
      case 15: // stmt -> attr_stmt
{ CurrentSemanticValue.sList = new(); }
        break;
      case 16: // stmt -> subgraph
{ CurrentSemanticValue.sList = ValueStack[ValueStack.Depth-1].sList; }
        break;
      case 17: // node_stmt -> node_id, opt_attr_list
{ CurrentSemanticValue.sList = MkVertexStmt(ValueStack[ValueStack.Depth-2].vertex, ValueStack[ValueStack.Depth-1].attributes); }
        break;
      case 18: // edge_stmt -> endpoint, edgeRHS, opt_attr_list
{ CurrentSemanticValue.sList = MkEdgeStmt(ValueStack[ValueStack.Depth-3].sList, ValueStack[ValueStack.Depth-2].sLists, ValueStack[ValueStack.Depth-1].attributes);}
        break;
      case 19: // endpoint -> node_id
{ CurrentSemanticValue.sList = MkSingleton(ValueStack[ValueStack.Depth-1].vertex); }
        break;
      case 20: // endpoint -> subgraph
{ CurrentSemanticValue.sList = ValueStack[ValueStack.Depth-1].sList; }
        break;
      case 21: // edgeRHS -> ARROW, endpoint
{ CurrentSemanticValue.sLists = MkSingleton(ValueStack[ValueStack.Depth-1].sList); }
        break;
      case 22: // edgeRHS -> edgeRHS, ARROW, endpoint
{ CurrentSemanticValue = ValueStack[ValueStack.Depth-3]; CurrentSemanticValue.sLists = new(CurrentSemanticValue.sLists, ValueStack[ValueStack.Depth-1].sList, ValueStack[ValueStack.Depth-1].sLists); }
        break;
      case 23: // subgraph -> '{', stmt_list, '}'
{ CurrentSemanticValue.sList = ValueStack[ValueStack.Depth-2].sList; }
        break;
      case 24: // subgraph -> SUBGRAPH, '{', stmt_list, '}'
{ CurrentSemanticValue.sList = ValueStack[ValueStack.Depth-2].sList;}
        break;
      case 25: // Anon@1 -> /* empty */
{CreateNewCurrentSubgraph(ValueStack[ValueStack.Depth-1].sVal); }
        break;
      case 26: // subgraph -> SUBGRAPH, id, Anon@1, '{', stmt_list, '}'
{ CurrentSemanticValue.sList = new(); 
		                        PopCurrentSubgraph(); }
        break;
      case 27: // subgraph -> SUBGRAPH
{ CurrentSemanticValue.sList = new(); }
        break;
      case 28: // attr_stmt -> GRAPH, attr_list
{ MkGraphAttrStmt(ValueStack[ValueStack.Depth-1].attributes); }
        break;
      case 29: // attr_stmt -> NODE, attr_list
{ MkVertexAttrStmt(ValueStack[ValueStack.Depth-1].attributes); }
        break;
      case 30: // attr_stmt -> EDGE, attr_list
{ MkEdgeAttrStmt(ValueStack[ValueStack.Depth-1].attributes); }
        break;
      case 31: // opt_attr_list -> /* empty */
{ CurrentSemanticValue.attributes = new(); }
        break;
      case 32: // opt_attr_list -> attr_list
{ CurrentSemanticValue = ValueStack[ValueStack.Depth-1]; }
        break;
      case 33: // attr_list -> '[', ']'
{ CurrentSemanticValue.attributes = new(); }
        break;
      case 34: // attr_list -> '[', a_list, ']'
{ CurrentSemanticValue = ValueStack[ValueStack.Depth-2]; }
        break;
      case 35: // a_list -> avPair
{ CurrentSemanticValue.attributes = new(); CurrentSemanticValue.attributes.Add(ValueStack[ValueStack.Depth-1].avPair); }
        break;
      case 36: // a_list -> avPair, a_list
{ ValueStack[ValueStack.Depth-1].attributes!.Add(ValueStack[ValueStack.Depth-2].avPair); CurrentSemanticValue = ValueStack[ValueStack.Depth-1]; }
        break;
      case 37: // a_list -> avPair, ',', a_list
{ ValueStack[ValueStack.Depth-1].attributes!.Add(ValueStack[ValueStack.Depth-3].avPair); CurrentSemanticValue = ValueStack[ValueStack.Depth-1]; }
        break;
      case 38: // avPair -> id, '=', id
{ CurrentSemanticValue.avPair = MkAvPair(ValueStack[ValueStack.Depth-3].sVal!, ValueStack[ValueStack.Depth-1].sVal!); }
        break;
      case 39: // avPair -> id
{ CurrentSemanticValue.avPair = MkAvPair(ValueStack[ValueStack.Depth-1].sVal!, ""); }
        break;
      case 40: // node_id -> id, opt_port
{ CurrentSemanticValue.vertex = MkVertex(ValueStack[ValueStack.Depth-2].sVal!); /* ignore port */ }
        break;
      case 41: // opt_port -> /* empty */
{}
        break;
      case 42: // opt_port -> port
{}
        break;
      case 43: // port -> port_location
{}
        break;
      case 44: // port -> port_angle
{}
        break;
      case 45: // port -> port_angle, port_location
{}
        break;
      case 46: // port -> port_location, port_angle
{}
        break;
      case 47: // port_location -> ':', id
{}
        break;
      case 48: // port_location -> ':', '(', id, ',', id, ')'
{}
        break;
      case 49: // id -> ID
{ CurrentSemanticValue.sVal = ValueStack[ValueStack.Depth-1].sVal; }
        break;
      case 50: // port_angle -> '@', id
{ CurrentSemanticValue.sVal = ValueStack[ValueStack.Depth-1].sVal; }
        break;
    }
#pragma warning restore 162, 1522
  }

  protected override string TerminalToString(int terminal)
  {
    if (aliases != null && aliases.ContainsKey(terminal))
        return aliases[terminal];
    else if (((DotTokens)terminal).ToString() != terminal.ToString(CultureInfo.InvariantCulture))
        return ((DotTokens)terminal).ToString();
    else
        return CharToString((char)terminal);
  }


/*

port : ':' ID [ ':' compass_pt ] 
     | ':' compass_pt 
subgraph : [ subgraph [ ID ] ] '{' stmt_list '}' 
compass_pt : (n | ne | e | se | s | sw | w | nw | c | _) 

The keywords node, edge, graph, digraph, subgraph, and strict are case-independent. 
Note also that the allowed compass point values are not keywords,
so these strings can be used elsewhere as ordinary identifiers and, conversely, 
the parser will actually accept any identifier. 

An ID is one of the following: 

Any string of alphabetic ([a-zA-Z\200-\377]) characters, underscores ('_') or digits ([0-9]), not beginning with a digit; 
a numeral [-]?(.[0-9]+ | [0-9]+(.[0-9]*)? ); 
any double-quoted string ("...") possibly containing escaped quotes (\")1; 
an HTML string (<...>). 
An ID is just a string; the lack of quote characters in the first two forms is just for simplicity. There is no semantic difference between abc_2 and "abc_2", or between 2.34 and "2.34". Obviously, to use a keyword as an ID, it must be quoted. Note that, in HTML strings, angle brackets must occur in matched pairs, and unescaped newlines are allowed. In addition, the content must be legal XML, so that the special XML escape sequences for ", &, <, and > may be necessary in order to embed these characters in attribute values or raw text. 
Both quoted strings and HTML strings are scanned as a unit, so any embedded comments will be treated as part of the strings. 

An edgeop is -> in directed graphs and -- in undirected graphs. 

An a_list clause of the form ID is equivalent to ID=true. 

The language supports C++-style comments: and //. In addition, a line beginning with a '#' character is considered a line output from a C preprocessor (e.g., # 34 to indicate line 34 ) and discarded. 

Semicolons aid readability but are not required except in the rare case that a named subgraph with no body immediately preceeds an anonymous subgraph, since the precedence rules cause this sequence to be parsed as a subgraph with a heading and a body. Also, any amount of whitespace may be inserted between terminals. 

As another aid for readability, dot allows single logical lines to span multiple physical lines using the standard C convention of a backslash immediately preceding a newline character. In addition, double-quoted strings can be concatenated using a '+' operator. As HTML strings can contain newline characters, they do not support the concatenation operator. 

*/
}
}
