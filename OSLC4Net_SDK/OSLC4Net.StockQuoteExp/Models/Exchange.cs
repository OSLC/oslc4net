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
using System.Web;

namespace OSLC4Net.StockQuoteSample.Models
{
    public enum Exchange
    {
        NASDAQ,
        NYSE
    }

    class ExchangeExtension
    {
        public static string ToString(Exchange exchange)
        {
            return exchange.ToString();
        }

        public static Exchange FromString(string value)
        {
            foreach (Exchange exchange in Enum.GetValues(typeof(Exchange)))
            {
                string stringValue = ToString(exchange);

                if (stringValue.Equals(value))
                {
                    return exchange;
                }
            }

            throw new ArgumentException();
        }
    }
}