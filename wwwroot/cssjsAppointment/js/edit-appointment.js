document.addEventListener('DOMContentLoaded', function () {
    // Thú cưng
    const btnAddPet = document.getElementById('btnAddPet');
    const dropdownPetList = document.getElementById('dropdownPetList');
    const petDropdown = dropdownPetList?.nextElementSibling;
    if (btnAddPet && dropdownPetList && petDropdown) {
        btnAddPet.addEventListener('click', function () {
            dropdownPetList.style.display = 'inline-block';
        });
        petDropdown.querySelectorAll('.add-pet-item').forEach(function (el) {
            el.addEventListener('click', function (e) {
                e.preventDefault();
                const petId = this.getAttribute('data-pet-id');
                const form = this.closest('form');
                const input = document.createElement('input');
                input.type = 'hidden';
                input.name = 'SelectedPetIds';
                input.value = petId;
                form.appendChild(input);
                form.submit();
            });
        });
    }
    document.querySelectorAll('.btn-remove-pet').forEach(function (el) {
        el.addEventListener('click', function () {
            const petId = this.getAttribute('data-pet-id');
            const form = this.closest('form');
            const input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'RemovePetId';
            input.value = petId;
            form.appendChild(input);
            form.submit();
        });
    });
    // Dịch vụ
    const btnAddService = document.getElementById('btnAddService');
    const dropdownServiceList = document.getElementById('dropdownServiceList');
    const serviceDropdown = dropdownServiceList?.nextElementSibling;
    if (btnAddService && dropdownServiceList && serviceDropdown) {
        btnAddService.addEventListener('click', function () {
            dropdownServiceList.style.display = 'inline-block';
        });
        serviceDropdown.querySelectorAll('.add-service-item').forEach(function (el) {
            el.addEventListener('click', function (e) {
                e.preventDefault();
                const serviceId = this.getAttribute('data-service-id');
                const form = this.closest('form');
                const input = document.createElement('input');
                input.type = 'hidden';
                input.name = 'SelectedServiceIds';
                input.value = serviceId;
                form.appendChild(input);
                form.submit();
            });
        });
    }
    document.querySelectorAll('.btn-remove-service').forEach(function (el) {
        el.addEventListener('click', function () {
            const serviceId = this.getAttribute('data-service-id');
            const form = this.closest('form');
            const input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'RemoveServiceId';
            input.value = serviceId;
            form.appendChild(input);
            form.submit();
        });
    });
});
