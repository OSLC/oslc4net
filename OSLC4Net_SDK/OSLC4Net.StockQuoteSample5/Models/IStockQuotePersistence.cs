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

namespace OSLC4Net.StockQuoteSample.Models
{
    /// <summary>
    /// Interface to store, retrieve, update and delete StockQuote objects
    /// </summary>
    interface IStockQuotePersistence
    {
        IEnumerable<StockQuote> GetAll();
        StockQuote Get(string tickerSymbol);
        StockQuote Add(StockQuote stockQuote);
        bool Update(StockQuote stockQuote);
        void Delete(string tickerSymbol);

    }
}
