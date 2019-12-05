/*******************************************************************************
 * Copyright (c) 2013 IBM Corporation.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * and Eclipse Distribution License v. 1.0 which accompanies this distribution.
 *  
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 * and the Eclipse Distribution License is available at
 * http://www.eclipse.org/org/documents/edl-v10.php.
 *
 * Contributors:
 *     Steve Pitschke  - initial API and implementation
 *******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime.Tree;

namespace OSLC4Net.Core.Query.Impl
{
    /// <summary>
    /// implmentation of ComparisonTerm interface
    /// </summary>
    internal class ComparisonTermImpl : SimpleTermImpl, ComparisonTerm
    {
        public
        ComparisonTermImpl(
            CommonTree tree,
            IDictionary<string, string> prefixMap
        ) : base(tree, TermType.COMPARISON, prefixMap)
        {
            switch (((CommonTree)tree.GetChild(1)).Token.Type) {
            case OslcWhereParser.EQUAL:
                op = Operator.EQUALS;
                break;
            case OslcWhereParser.NOT_EQUAL:
                op = Operator.NOT_EQUALS;
                break;
            case OslcWhereParser.LESS:
                op = Operator.LESS_THAN;
                break;
            case OslcWhereParser.LESS_EQUAL:
                op = Operator.LESS_EQUALS;
                break;
            case OslcWhereParser.GREATER:
                op = Operator.GREATER_THAN;
                break;
            default:
            case OslcWhereParser.GREATER_EQUAL:
                op = Operator.GREATER_EQUALS;
                break;
            }
        }

        public Operator Operator
        {
            get
            {
                return op;
            }
        }

        public IValue Operand
        {
            get
            {
                if (operand == null)
                {
                    CommonTree treeOperand = (CommonTree)tree.GetChild(2);
            
                    operand = CreateValue(treeOperand, "unspported literal value type",
                                          prefixMap);            
                }

                return operand;
            }
        }

        public override string ToString()
        {
            return Property.ToString() + OperatorExtension.ToString(op) + Operand.ToString();
        }
    
        static internal IValue
        CreateValue(
            CommonTree treeOperand,
            string errorPrefix,
            IDictionary<string, string> prefixMap
        )
        {
            switch (treeOperand.Token.Type) {
            case OslcWhereParser.IRI_REF:
                return new UriRefValueImpl(treeOperand);
            case OslcWhereParser.BOOLEAN:
                return new BooleanValueImpl(treeOperand);
            case OslcWhereParser.DECIMAL:
                return new DecimalValueImpl(treeOperand);
            case OslcWhereParser.STRING_LITERAL:
                return new StringValueImpl(treeOperand);
            case OslcWhereParser.TYPED_VALUE:
                return new TypedValueImpl(treeOperand, prefixMap);
            case OslcWhereParser.LANGED_VALUE:
                return new LangedStringValueImpl(treeOperand);
            default:
                throw new InvalidOperationException(
                        errorPrefix + ": " +
                            treeOperand.Token.Text);
            }       
        }
    
        private readonly Operator op;
        private IValue operand = null;
    }

    internal static class OperatorExtension
    {
        public static string
        ToString(Operator op)
        {
            switch (op)
            {
                case Operator.EQUALS:
                    return "=";
                case Operator.NOT_EQUALS:
                    return "!=";
                case Operator.LESS_THAN:
                    return "<";
                case Operator.GREATER_THAN:
                    return ">";
                case Operator.LESS_EQUALS:
                    return "<=";
                default:
                case Operator.GREATER_EQUALS:
                    return ">=";
            }
        }
    }
}
