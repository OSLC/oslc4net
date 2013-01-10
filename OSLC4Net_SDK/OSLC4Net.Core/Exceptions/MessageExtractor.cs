/*******************************************************************************
 * Copyright (c) 2012 IBM Corporation.
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
using System.Resources;
using System.Text;

using log4net;

using OSLC4Net.Core.Properties;

namespace OSLC4Net.Core.Exceptions
{
    public static class MessageExtractor
    {
        private static ResourceManager rm = Resources.ResourceManager;
        private static readonly ILog logger = LogManager.GetLogger(typeof(MessageExtractor));

        public static String GetMessage(String key, Object[] args)
        {

            try {
                String message = rm.GetString( key );
                return String.Format(message, args);
            } catch (Exception missingResourceException ) {
                logger.Fatal(missingResourceException.Message, missingResourceException);
                return "???" + key + "???";
            }
        }
    }
}
