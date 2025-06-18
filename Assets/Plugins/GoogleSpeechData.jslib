mergeInto(LibraryManager.library, {
    Recording: function() {
    navigator.mediaDevices.getUserMedia({ audio: true })
        ...
            ...
         	// ArrayBuffer를 Base64로 변환하는 함수
            function arrayBufferToBase64(buffer) {
                let binary = '';
                const bytes = new Uint8Array(buffer);
                const len = bytes.byteLength;
                for (let i = 0; i < len; i++) {
                    binary += String.fromCharCode(bytes[i]);
                }
                return btoa(binary);
            };
			...

           mediaRecorder.onstop = function() {
                const audioBlob = new Blob(audioChunks, { type: 'audio/wav' });

				 // WAV 파일을 ArrayBuffer로 읽기
                const reader = new FileReader();
                reader.readAsArrayBuffer(audioBlob);
                reader.onloadend = function() {
                    // ArrayBuffer를 Uint8Array로 변환 (WAV 파일 전체)
                    const wavData = new Uint8Array(reader.result);
                    //Base64로 변환
                    const base64Wav = arrayBufferToBase64(wavData.buffer);
                    //Unity로 데이터 전송
                    SendMessage('GameManager', 'ReceiveAudio', base64Wav);
				...
},
...

});