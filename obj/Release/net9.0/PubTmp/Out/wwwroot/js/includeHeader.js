// js/include.js
function includeHTML() {
  document.querySelectorAll('[include-html]').forEach(el => {
    const file = el.getAttribute('include-html');
    fetch(file)
      .then(res => res.ok ? res.text() : Promise.reject("Không tìm thấy"))
      .then(data => {
        el.innerHTML = data;
        el.removeAttribute('include-html');
        includeHTML(); // xử lý đệ quy nếu có include lồng nhau
      })
      .catch(err => el.innerHTML = "Lỗi khi tải file");
  });
}

document.addEventListener("DOMContentLoaded", includeHTML);
