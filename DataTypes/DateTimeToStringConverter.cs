/*******************************************************************
 Copyright 2024 CornellPad

 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at

 http://www.apache.org/licenses/LICENSE-2.0

 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
 or implied. See the License for the specific language governing
 permissions and limitations under the License.
 *******************************************************************/

using System.Globalization;

namespace CornellPad.DataTypes;

public class DateTimeToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            var currentApp = Application.Current as CornellPad.App;
            if (currentApp != null)
            {
                string formatString;
                switch (currentApp.GetCurrentTheme())
                {
                    case CornellPadTheme.VI:
                        formatString = "M/dd/yyyy\nh:mm tt";
                        break;
                    default:
                        formatString = "M/dd/yyyy h:mm tt";
                        break;
                }
                
                return dateTime.ToString(formatString);
            }
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
