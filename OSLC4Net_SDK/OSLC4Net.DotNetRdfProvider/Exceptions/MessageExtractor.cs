/*******************************************************************************
 * Copyright (c) 2012, 2013 IBM Corporation.
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

namespace OSLC4Net.Core.Exceptions
{

    using System;
    using System.Resources;

    using log4net;

    using OSLC4Net.Client.Properties;

    /// <summary>
    /// Utility methods for retrieving messages
    /// </summary>
    public static class MessageExtractor
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MessageExtractor));
        private static ResourceManager rm = Resources.ResourceManager;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GetMessage(string key, object[] args)
        {

            try
            {
                string message = rm.GetString(key);
                return string.Format(message, args);
            }
            catch (Exception missingResourceException)
            {
                Logger.Fatal(missingResourceException.Message, missingResourceException);
                return "???" + key + "???";
            }
        }
    }
}
