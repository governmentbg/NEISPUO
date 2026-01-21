export const nomenclatureItems =  [
    { title: 'Вид студент', name: 'StudentType', icon: 'fas fa-user-graduate' },                    
    { title: 'Тип ресурсен специалист', name: 'ResourceSupportSpecialistType', icon: 'settings' },
    { title: 'Вид пътуване', name: 'CommuterType', icon: 'fas fa-bus' },
    { title: 'Основание за отпускане на стипендия', name: 'ScholarshipType', icon: 'fas fa-coins' },                            
    { title: 'Специални потребности', name: 'SpecialNeedsType', icon: 'fas fa-coins' },     
    { title: 'Подтип Специални потребности', name: 'SpecialNeedsSubType', icon: 'fas fa-coins' },
    { title: 'Вид ресурсно подпомагане', name: 'ResourceSupportType', icon: 'fas fa-coins' },     
    { title: 'Вид специалист ресурсно подпомагане', name: 'ResourceSupportSpecialistType', icon: 'fas fa-coins' },
    { title: 'Вид обща подкрепа', name: 'CommonSupportType', icon: 'fas fa-coins' },
    { title: 'Вид допълнителна подкрепа', name: 'AdditionalSupportType', icon: 'fas fa-coins' },
    { title: 'Основание за записване', name: 'AdmissionReasonType', icon: 'fas fa-coins' },
    { title: 'Основание за отписване', name: 'DischargeReasonType', icon: 'fas fa-coins' },
    { title: 'Езици', name: 'Language', icon: 'fas fa-language' }                              
];

export default {
    findName: (name) => {
        let arr = nomenclatureItems.filter(item => item.name == name);
        if (arr.length > 0) return arr[0];
        else return null;
    }
};
