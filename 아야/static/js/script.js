$(document).ready(function () {
    $("#analyze-btn").click(function () {
      const patient_input = $("#symptoms").val();
      if (!patient_input) {
        alert("증상을 입력해주세요!");
        return;
      }
  
      // Flask 서버에 AJAX 요청
      $.ajax({
        url: "/predict",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify({ patient_input: patient_input }),
        success: function (response) {
          // 서버에서 받은 결과를 모달에 표시
          const results = response.results || [];
          let resultHtml = "";
          results.forEach((result, index) => {
            resultHtml += `<p><strong>${index + 1}. 질병 이름:</strong> ${result.disease_name} <br>
                           <strong>일치하는 증상:</strong> ${result.symptoms} <br>
                           <strong>치료 방법:</strong> ${result.treatments} <br>
                           <strong>확률:</strong> ${result.confidence}</p><hr>`;
          });
          $("#result-details").html(resultHtml);
          $("#result-modal").fadeIn();
        },
        error: function (xhr) {
          alert("오류 발생: " + (xhr.responseJSON?.error || "알 수 없는 오류"));
        },
      });
    });
  
    // 모달 닫기 기능
    $("#close-modal").click(function () {
      $("#result-modal").fadeOut();
    });
  });
  