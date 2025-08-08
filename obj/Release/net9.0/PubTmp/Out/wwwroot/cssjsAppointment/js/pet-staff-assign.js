// Script động cập nhật bảng phân công staff cho từng pet
$(document).ready(function () {
    // Lưu lại staff list để render dropdown
    var staffList = [];
    if (window.PET_STAFF_ASSIGN_STAFFS) {
        staffList = window.PET_STAFF_ASSIGN_STAFFS;
    } else if (typeof staffsJson !== 'undefined') {
        staffList = staffsJson;
    }

    // Render bảng phân công staff cho pet
    function renderPetStaffTable(pets, petStaffAssignments) {
        var html = '';
        if (!pets || pets.length === 0) {
            html = '<div class="alert alert-info">Vui lòng chọn thú cưng để phân công nhân viên.</div>';
            $('#pet-staff-assign-table').html(html);
            return;
        }
        html += '<div class="table-responsive"><table class="table table-bordered align-middle mb-0">';
        html += '<thead class="table-light"><tr><th style="width:40%">Thông tin thú cưng</th><th style="width:60%">Nhân viên phụ trách</th></tr></thead><tbody>';
        pets.forEach(function(pet, i) {
            var assign = petStaffAssignments.find(x => x.PetId == pet.PetId) || {};
            html += '<tr>';
            html += '<td>';
            html += '<input type="hidden" name="PetStaffAssignments['+i+'].PetId" value="'+pet.PetId+'" />';
            html += '<div class="d-flex align-items-center gap-2">';
            html += '<span style="display:inline-block;width:40px;height:40px;border-radius:50%;background:#f0f0f0;border:1px solid #ddd;text-align:center;line-height:40px;font-size:20px;color:#bbb;"><i class="fas fa-dog"></i></span>';
            html += '<div><div class="fw-bold">'+pet.Name+'</div>';
            if (pet.Breed) html += '<div class="text-muted small">Giống: '+pet.Breed+'</div>';
            html += '</div></div></td>';
            html += '<td>';
            html += '<select name="PetStaffAssignments['+i+'].StaffId" class="form-select select2-staff w-100" style="width:100%">';
            html += '<option value="">-- Chọn nhân viên --</option>';
            staffList.forEach(function(staff) {
                var selected = assign.StaffId == staff.Value ? ' selected' : '';
                html += '<option value="'+staff.Value+'"'+selected+'>'+staff.Text+'</option>';
            });
            html += '</select></td></tr>';
        });
        html += '</tbody></table></div>';
        $('#pet-staff-assign-table').html(html);
        $('.select2-staff').select2({
            placeholder: 'Tìm nhân viên theo tên, SĐT, email',
            allowClear: true,
            minimumInputLength: 0,
            ajax: {
                url: '/AdminAppointment/SearchStaffs',
                dataType: 'json',
                delay: 250,
                data: function (params) { return {term: params.term}; },
                processResults: function (data) { return {results: data.results}; },
                cache: true
            }
        });
    }

    // Khi thay đổi select2-pet
    $('.select2-pet').on('change', function () {
        var selectedPets = $(this).select2('data').map(function(p) {
            return { PetId: p.id, Name: p.text, Breed: p.breed || '' };
        });
        // Lấy lại các staff đã gán trước đó nếu có
        var petStaffAssignments = [];
        $('#pet-staff-assign-table input[type=hidden][name$=PetId]').each(function(idx, el) {
            var petId = $(el).val();
            var staffId = $(el).closest('tr').find('select').val();
            petStaffAssignments.push({ PetId: petId, StaffId: staffId });
        });
        renderPetStaffTable(selectedPets, petStaffAssignments);
    });

    // Khởi tạo lần đầu nếu có dữ liệu
    if (window.PET_STAFF_ASSIGN_PETS) {
        renderPetStaffTable(window.PET_STAFF_ASSIGN_PETS, window.PET_STAFF_ASSIGN_ASSIGNMENTS || []);
    }
});
