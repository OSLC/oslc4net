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
 *     Michael Fiedler  - initial API and implementation
 *******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OSLC4Net.Core.Attribute;
using OSLC4Net.Core.Model;

namespace OSLC4Net.StockQuoteSample.Models
{

    public static class Constants
    {
        public const string STOCK_QUOTE_DOMAIN = "http://yourcompany.com/ns/stockquote#";
        public const string STOCK_QUOTE_NAMESPACE = "http://yourcompany.com/ns/stockquote#";
        public const string STOCK_QUOTE_NAMESPACE_PREFIX = "stockquote";

        public const string STOCK_QUOTE = "StockQuote";
        public const string TYPE_STOCK_QUOTE = STOCK_QUOTE_NAMESPACE + STOCK_QUOTE ;

        public const string PATH_STOCK_QUOTE = "stockquote";
        public const string PATH_STOCK_QUOTE_SHAPE = "getShape=true";
    }
}
