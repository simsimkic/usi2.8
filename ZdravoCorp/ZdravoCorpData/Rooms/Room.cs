using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Repositories;
using ZdravoCorp.ZdravoCorpData.Examinations;

namespace ZdravoCorp.ZdravoCorpData.Rooms
{
    public class Room
    {
        public enum Type { EXAMINATION, OPERATION, WAITING, INPATIENT ,WAREHOUSE};
        public int id { get; set; }
        public Type type { get; set; }

        public Room(int id, Type type)
        {
            this.id = id;
            this.type = type;
        }

        public static int getAvailable(DateTime startInterval, DateTime endInterval,Room.Type roomType)
        {
            var examinationRooms = MainRepository.rooms.Where(r => r.type == roomType).ToList();

            foreach (var examinationRoom in examinationRooms)
            {
                var examinationIds = ExamRoom.getExaminationIds(examinationRoom.id);
                var unavailableTimeSlots = getUnavailableTimeSlots(examinationIds);

                var isAvailable = unavailableTimeSlots.All(slot =>
                    slot.start.AddMinutes((int)slot.duration.TimeSpan.TotalMinutes) <= startInterval ||
                    slot.start >= endInterval);

                if (isAvailable)
                {
                    return examinationRoom.id;
                }
            }

            return 0;
        }

        public static List<TimeSlot> getUnavailableTimeSlots(List<int> examinationIds)
        {
            List<TimeSlot>unavailable=new List<TimeSlot>();

            for (int i = 0; i < examinationIds.Count; i++)
            {
                unavailable.AddRange(MainRepository.examinations
                    .Where(e => e.id == examinationIds[i] && e.status!=Examination.Status.CANCELLED && e.status!=Examination.Status.FINISHED)
                    .Select(e => e.timeSlot)
                    .ToList());
            }
            return unavailable;

        }
        

        

    }
}
