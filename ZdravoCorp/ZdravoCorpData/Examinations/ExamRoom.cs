using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Repositories;

namespace ZdravoCorp.ZdravoCorpData.Examinations
{
    public class ExamRoom
    {
        public int examinationId { get; set; }
        public int roomId { get; set; }

        public ExamRoom(int examinationId, int roomId)
        {
            this.examinationId = examinationId;
            this.roomId = roomId;
        }

        public static int get(int examinationId)
        {
            ExamRoom? examRoom = MainRepository.examrooms.Where(e => e.examinationId == examinationId).FirstOrDefault();
            return examRoom.roomId;
        }

       public static List<int> getExaminationIds(int roomId)
        {
           return MainRepository.examrooms.Where(e => e.roomId == roomId).Select(e => e.examinationId).ToList();

        }
    }
}
