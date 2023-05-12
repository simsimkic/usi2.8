using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Repositories;

namespace ZdravoCorp.ZdravoCorpData.Examinations
{
    public class ExaminationSystem {

        private List<Admission> _admissions;

        public ExaminationSystem() {
            _admissions = MainRepository.admissions;
        }

        public Admission GetAdmission(int examinationId)
        {
            for (int i = 0; i < _admissions.Count; i++)
            {
                if (_admissions[i].examinationId == examinationId)
                {
                    return _admissions[i];
                }
            }
            return null;
        }
    }
}
