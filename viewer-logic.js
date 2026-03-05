var viewer;

var options = {
    env: 'AutodeskProduction2',
    api: 'streamingV2',
    getAccessToken: function (onTokenReady) {
        fetch('https://localhost:7244/api/auth/public-token')
            .then(response => {
                if (!response.ok) throw new Error('Auth failed: ' + response.status);
                return response.json();
            })
            .then(data => {
                onTokenReady(data.access_token, data.expires_in);
            })
            .catch(err => console.error('Error fetching token:', err));
    }
};

Autodesk.Viewing.Initializer(options, function () {
    var htmlDiv = document.getElementById('apsViewer');
    viewer = new Autodesk.Viewing.GuiViewer3D(htmlDiv);
    viewer.start();

    // Paste the base64 URN from your /translate response directly
    var documentId = 'model URN HERE';

    loadModel(documentId);
});

function loadModel(documentId) {
    Autodesk.Viewing.Document.load(documentId, onDocumentLoadSuccess, onDocumentLoadFailure);
}

function onDocumentLoadSuccess(doc) {
    // Get all viewables
    var viewables = doc.getRoot().search({ type: 'geometry', role: '3d' });

    if (viewables.length > 0) {
        // Load the first 3D view
        viewer.loadDocumentNode(doc, viewables[0]);
    } else {
        // Fallback to default if no 3D view found
        var defaultView = doc.getRoot().getDefaultGeometry();
        viewer.loadDocumentNode(doc, defaultView);
        console.warn('No 3D view found, loading default view');
    }
}
function onDocumentLoadFailure(viewerErrorCode) {
    console.error('onDocumentLoadFailure() - errorCode:' + viewerErrorCode);
}