using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ZdravoCorp.Repositories;

namespace ZdravoCorp.Patients
{
    public class Validations
    {

        public static bool CheckIfEmpty(string value)
        {
            if (value == String.Empty)
            {
                return true;
            }
            return false;
        }

        public static bool isValidInputForFind(TextBox date_txt, TextBox start_txt, 
                                                TextBox end_txt, ComboBox doctor_cbx, ComboBox priority_cbx)
        {
            bool isDateValid = !CheckIfEmpty(date_txt.Text);
            bool isStartTimeValid = !CheckIfEmpty(start_txt.Text);
            bool isEndTimeValid = !CheckIfEmpty(end_txt.Text);
            bool isDoctorSelected = doctor_cbx.SelectedIndex != -1;
            bool isPrioritySelected = priority_cbx.SelectedIndex != -1;

            return isDateValid && isStartTimeValid && isEndTimeValid && isDoctorSelected && isPrioritySelected;
        }

        public static List<DateTime> getValidFutureDateTimes(TextBox start_txt, TextBox end_txt)
        {
            DateTime startTime;
            DateTime endTime;

            // Parse start time
            if (!DateTime.TryParseExact(start_txt.Text, "HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out startTime))
            {
                MessageBox.Show("Invalid start time format. Please enter time in format 'HH:mm:ss'.");
                return null;
            }

            // Parse end time
            if (!DateTime.TryParseExact(end_txt.Text, "HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out endTime))
            {
                MessageBox.Show("Invalid end time format. Please enter time in format 'HH:mm:ss'.");
                return null;
            }

            // Check if end time is greater than start time
            if (endTime < startTime)
            {
                MessageBox.Show("End time cannot be less than start time.");
                return null;
            }

            return new List<DateTime>() { startTime, endTime };
        }

        public static DateTime getValidFutureDate(TextBox date_txt)
        {
            DateTime date;

            // Parse date
            if (!DateTime.TryParseExact(date_txt.Text, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out date))
            {
                MessageBox.Show("Invalid date format. Please enter date in format 'yyyy-MM-dd'.");
                return DateTime.MinValue;
            }

            // Check if date is before today
            if (date <= DateTime.Today)
            {
                MessageBox.Show("Date cannot be before today.");
                return DateTime.MinValue;
            }

            return date;
        }

        public static DateTime ParseStartTimeFromTextBox(string timeInput)
        {
            DateTime time;
            try
            {
                time = DateTime.ParseExact(timeInput, "HH:mm:ss", null);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while converting the date: " + ex.Message);
            }
            return time;
        }

        public static bool isDoctorAvailableAtTime(DateTime desiredTime, int doctorId)
        {
            var examinations = MainRepository.examinations;

            foreach (var examination in examinations)
            {
                if (doctorId != examination.doctorId)
                {
                    continue;
                }

                var examintionStart = examination.timeSlot.start;
                var durationInMinutes = (int)examination.timeSlot.duration.TimeSpan.TotalMinutes;
                var examinationEnd = addMinutesToDateTime(examintionStart, durationInMinutes);
                var time15MinutesBefore = addMinutesToDateTime(examintionStart, -durationInMinutes);

                if ((desiredTime >= time15MinutesBefore && desiredTime <= examintionStart) ||
                    (desiredTime >= examintionStart && desiredTime <= examinationEnd))
                {
                    return false;
                }
            }

            return true;
        }
        public static DateTime addMinutesToDateTime(DateTime examintionStart, int durationInMinutes)
        {
            return examintionStart.AddMinutes(durationInMinutes);
        }

        public static TimeSpan addMinutesToTime(TimeSpan examintionStart, int durationInMinutes)
        {
            return examintionStart.Add(TimeSpan.FromMinutes(durationInMinutes));
        }

    }
}
