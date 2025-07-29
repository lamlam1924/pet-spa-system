$(document).ready(function(){
    
    (function($) {
        "use strict";

    
    jQuery.validator.addMethod('answercheck', function (value, element) {
        return this.optional(element) || /^\bcat\b$/.test(value)
    }, "nhập câu trả lời đúng");

    // validate contactForm form
    $(function() {
        $('#contactForm').validate({
            rules: {
                name: {
                    required: true,
                    minlength: 2
                },
                subject: {
                    required: true,
                    minlength: 4
                },
                number: {
                    required: true,
                    minlength: 5
                },
                email: {
                    required: true,
                    email: true
                },
                message: {
                    required: true,
                    minlength: 20
                }
            },
            messages: {
                name: {
                    required: "Vui lòng nhập tên của bạn",
                    minlength: "Tên phải có ít nhất 2 ký tự"
                },
                subject: {
                    required: "Vui lòng nhập chủ đề",
                    minlength: "Chủ đề phải có ít nhất 4 ký tự"
                },
                number: {
                    required: "Vui lòng nhập số điện thoại",
                    minlength: "Số điện thoại phải có ít nhất 5 ký tự"
                },
                email: {
                    required: "Vui lòng nhập email"
                },
                message: {
                    required: "Vui lòng nhập nội dung tin nhắn",
                    minlength: "Nội dung phải có ít nhất 20 ký tự"
                }
            },
            submitHandler: function(form) {
                $(form).ajaxSubmit({
                    type:"POST",
                    data: $(form).serialize(),
                    url:"contact_process.php",
                    success: function() {
                        $('#contactForm :input').attr('disabled', 'disabled');
                        $('#contactForm').fadeTo( "slow", 1, function() {
                            $(this).find(':input').attr('disabled', 'disabled');
                            $(this).find('label').css('cursor','default');
                            $('#success').fadeIn()
                            $('.modal').modal('hide');
		                	$('#success').modal('show');
                        })
                    },
                    error: function() {
                        $('#contactForm').fadeTo( "slow", 1, function() {
                            $('#error').fadeIn()
                            $('.modal').modal('hide');
		                	$('#error').modal('show');
                        })
                    }
                })
            }
        })
    })
        
 })(jQuery)
})